using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextComponent : MonoBehaviour
{
    public string Text;
    public int Layer;
    public Color TextureColor;
    public bool Inverted;
    public bool RightToLeft;
    public bool InvertText;
    public float CharDistance;
    public bool Translate;
    public Vector2 Offset;
    public Vector2 Scale;
    public DialogueArrow DialogueArrow;
    public bool AutoBlinkDialogueArrow;
    public float TimeBetweenCharacters;

    //public int CharactersPerLine;
    public TextDisposition Disposition;
    [Header("Sound")]
    public AudioSource SoundOnCharacter;
    public bool SoundEnabled;
    public int PlaySoundEachXCharacters;
    [Header("Spin Effect")]
    public int AmountOfCycles;
    public int AnglesPerFrame;

    private Sprite[] fontSpritesArray;
    private Dictionary<char, Sprite> fontSprites;
    private string innerText;
    private int charactersPerLine;
    private RectTransform rectTransform;
    private Coroutine mainTextCoroutine;
    public bool stopText;
    private bool isProcessingText;

    [Header("Option")]
    public SpriteRenderer OptionSelect;
    private SpriteRenderer generatedOptionSelect;
    public BoxCollider2D optionCollider;
    public ColliderMouseOver optionMouseOver;
    private Vector2 optionStart = Vector2.zero;
    private Vector2 optionEnd = Vector2.zero;

    public bool IsFullyRendered { get; private set; }

    public void Restart()
    {
        isProcessingText = stopText = false;
        this.innerText = null;
        foreach (Transform glyph in this.transform)
        {
            Destroy(glyph.gameObject);
        }
    }

    public enum TextDisposition
    {
        SingleLine,
        MultiLine,
        MultiLineHyphenated
    }

    private void Awake()
    {
        this.fontSpritesArray = this.Inverted ? Resources.LoadAll<Sprite>("Sprites/font") : Resources.LoadAll<Sprite>("Sprites/font");
        fontSprites = this.fontSpritesArray.ToDictionary(spr => GetSymbolTranslation(spr.name), spr => spr);
    }

    private void Start()
    {
        this.rectTransform = this.GetComponent<RectTransform>();
        this.charactersPerLine = int.MaxValue;
        if (rectTransform != null)
        {
            this.charactersPerLine = (int)this.rectTransform.sizeDelta.x / (int)((this.fontSprites.First().Value.bounds.size.x + this.CharDistance) * (this.Scale.x > 0 ? this.Scale.x : 1));
            if (this.charactersPerLine == 0) this.charactersPerLine = 1;
        }

        mainTextCoroutine = StartCoroutine(UpdateText());
        this.innerText = Text;
    }

    public IEnumerator CheckOptionSelect()
    {
        while (true)
        {
            if (generatedOptionSelect != null && this.enabled)
            {
                generatedOptionSelect.enabled = optionMouseOver.IsOverlapping;
            }
            yield return 1;
        }
    }

    private void Update()
    {
        if (!this.isActiveAndEnabled)
        {
            return;
        }

        if (innerText != Text)
        {
            IsFullyRendered = false;
            innerText = Text;
            mainTextCoroutine = StartCoroutine(UpdateText());
        }
    }

    private void OnDisable()
    {
        Restart();
        stopText = true;
        if (!this.isActiveAndEnabled)
        {
            return;
        }
        this.Text = "";
        mainTextCoroutine = StartCoroutine(UpdateText());
    }

    public void SetColor(Color color)
    {
        this.TextureColor = color;
    }

    public IEnumerator SetText(string text)
    {
        IsFullyRendered = false;
        this.innerText = this.Text = text;
        yield return UpdateText();
    }

    private IEnumerator UpdateText()
    {
        while (isProcessingText)
        {
            stopText = true;
            yield return 1;
        }

        stopText = false;
        isProcessingText = true;

        if (!this.isActiveAndEnabled)
        {
            yield break;
        }

        if (DialogueArrow != null && AutoBlinkDialogueArrow) DialogueArrow.IsBlinking = false;

        foreach (Transform glyph in this.transform)
        {
            Destroy(glyph.gameObject);
        }

        var charList = (InvertText ? this.Text.Reverse() : this.Text).ToArray();

        switch (this.Disposition)
        {
            case TextDisposition.SingleLine:
                yield return SetTextSingleLine(charList, 0);
                break;

            case TextDisposition.MultiLine:
                yield return SetTextMultiLine(charList, 0);
                break;

            case TextDisposition.MultiLineHyphenated:
                yield return SetTextMultiLineHyphenated(charList, 0);
                break;
        }
        isProcessingText = false;
        mainTextCoroutine = null;
    }

    public IEnumerator AppendText(string text)
    {
        string fullText = (this.innerText + text);
        var charList = (InvertText ? fullText.Reverse() : fullText).ToArray();

        switch (this.Disposition)
        {
            case TextDisposition.SingleLine:
                yield return SetTextSingleLine(charList, innerText.Length);
                break;

            case TextDisposition.MultiLine:
                yield return SetTextMultiLine(charList, innerText.Length);
                break;

            case TextDisposition.MultiLineHyphenated:
                yield return SetTextMultiLineHyphenated(charList, innerText.Length);
                break;
        }

        this.innerText = this.Text = this.innerText + text;
    }

    private IEnumerator FillTextSingleLine(char[] text, int lineOffset = 0, int initialCharOffset = 0)
    {
        int soundplay = 0;
        Vector2 offset = Vector2.zero;
        for (int i = initialCharOffset; i < text.Length; i++)
        {
            if (stopText) yield break;
            if (i < 0) continue;
            char c = text[i];
            if (c == ' ')
            {
                continue;
            }

            SpriteRenderer renderer = CreateGlyph(c);
            offset.x = (i * renderer.sprite.rect.size.x + this.CharDistance * i) * (this.Scale.x > 0 ? this.Scale.x : 1) * (RightToLeft ? -1 : 1);
            offset.y = lineOffset * -renderer.sprite.rect.size.y * (this.Scale.x > 0 ? this.Scale.x : 1);
            renderer.transform.localPosition = new Vector3(this.Offset.x + offset.x - this.rectTransform.sizeDelta.x / 2 + renderer.sprite.rect.size.x / 2,
                                                           this.Offset.y + offset.y + this.rectTransform.sizeDelta.y / 2 - renderer.sprite.rect.size.y / 2, 0);

            if (i == 0)
            {
                optionStart = renderer.transform.localPosition;
            }

            optionEnd = renderer.transform.localPosition;           

            if (PlaySoundEachXCharacters == 0) PlaySoundEachXCharacters = 2;
            if (SoundEnabled && SoundOnCharacter != null && soundplay % PlaySoundEachXCharacters == 0)
            {
                SoundOnCharacter.pitch = 0.9f + (Random.value / 6f);
                SoundOnCharacter.Play();
            }
            soundplay++;
            yield return new WaitForSeconds(TimeBetweenCharacters);
        }
        RenderOption(lineOffset);
    }

    private IEnumerator SetTextSingleLine(char[] text, int initialCharOffset)
    {
        yield return FillTextSingleLine(text, initialCharOffset: initialCharOffset);
        if (DialogueArrow != null && AutoBlinkDialogueArrow) DialogueArrow.IsBlinking = true;
        IsFullyRendered = true;
    }

    private IEnumerator SetTextMultiLine(char[] text, int initialCharOffset)
    {
        char[][] lines = text.GroupByWords(this.charactersPerLine).Select(h => h.ToArray()).ToArray();
        int lineNumber = 0;
        int totalOffset = initialCharOffset;
        foreach (char[] line in lines)
        {
            if (totalOffset > line.Length)
            {
                totalOffset -= Mathf.Min(line.Length + 1, totalOffset);
                lineNumber++;
                continue;
            }
            yield return FillTextSingleLine(line, lineNumber, totalOffset);
            totalOffset -= line.Length;
            lineNumber++;
        }
        if (DialogueArrow != null && AutoBlinkDialogueArrow) DialogueArrow.IsBlinking = true;
        IsFullyRendered = true;
    }

    private IEnumerator SetTextMultiLineHyphenated(char[] text, int initialCharOffset)
    {
        char[][] lines = text.HyphenateAllLines(this.charactersPerLine).Select(h => h.ToArray()).ToArray();
        int lineNumber = 0;
        int totalOffset = initialCharOffset;
        foreach (char[] line in lines)
        {
            if (totalOffset > line.Length)
            {
                totalOffset -= Mathf.Min(line.Length, totalOffset);
                continue;
            }
            yield return FillTextSingleLine(line, lineNumber, totalOffset);
            totalOffset -= line.Length;
            lineNumber++;
        }
        if (DialogueArrow != null && AutoBlinkDialogueArrow) DialogueArrow.IsBlinking = true;
        IsFullyRendered = true;
    }

    private SpriteRenderer CreateGlyph(char c)
    {
        GameObject glyph = new GameObject("Glyph_" + c.ToString());
        glyph.layer = this.gameObject.layer;
        glyph.transform.parent = this.transform;
        glyph.transform.localScale = this.Scale == Vector2.zero ? Vector3.one : new Vector3(this.Scale.x, this.Scale.y);

        SpinFadeIn fadeIn = glyph.AddComponent<SpinFadeIn>();
        fadeIn.amountOfCycles = AmountOfCycles;
        fadeIn.anglesPerFrame = AnglesPerFrame;

        SpriteRenderer renderer = glyph.AddComponent<SpriteRenderer>();
        renderer.sprite = fontSprites[c];
        renderer.transform.parent = glyph.transform;
        renderer.sortingOrder = this.Layer;
        renderer.material.SetColor("_Color", this.TextureColor);        

        return renderer;
    }

    private void RenderOption(int lineOffset)
    {
        if (OptionSelect != null)
        {
            GameObject select = new GameObject("Select_"+ lineOffset);
            select.transform.parent = this.gameObject.transform;
            select.layer = this.gameObject.layer;
            SpriteRenderer optionRenderer = select.AddComponent<SpriteRenderer>();
            optionRenderer.sprite = OptionSelect.sprite;
            optionRenderer.transform.localScale = new Vector3(OptionSelect.transform.localScale.x * (optionEnd.x+ 20f - optionStart.x) / 32f, OptionSelect.transform.localScale.y, OptionSelect.transform.localScale.z);
            optionRenderer.transform.localPosition = optionStart + (optionEnd - optionStart) / 2f;
            optionRenderer.sortingOrder = this.Layer - 1;
            optionRenderer.material = Instantiate(OptionSelect.sharedMaterial);
            optionRenderer.enabled = false;
            generatedOptionSelect = optionRenderer;

            optionCollider.size = optionRenderer.transform.localScale * 256f;
            optionCollider.offset = optionRenderer.transform.localPosition;

            StartCoroutine(CheckOptionSelect());
        }
    }

    private char GetSymbolTranslation(string symbol)
    {
        if (symbol == null) return '0';
        if (symbol.Length == 1)
        {
            return symbol.Single();
        }
        if (symbol.StartsWith("Num"))
        {
            return symbol.SkipWhile(c => c != '_').Skip(1).Take(1).FirstOrDefault();
        }
        if (symbol.StartsWith("Sym"))
        {
            return TransformSymbolToSign(symbol.SkipWhile(c => c != '_').Skip(1));
        }
        return '0';
    }

    private char TransformSymbolToSign(IEnumerable<char> symbol)
    {
        switch (new string(symbol.ToArray()))
        {
            case "Colon":
                return ':';

            case "Exclamation":
                return '!';

            case "Fullstop":
                return '.';

            case "Minus":
                return '-';

            case "Plus":
                return '+';

            case "Question":
                return '?';

            case "Comma":
                return ',';

            case "Apostrophe":
                return '\'';

            case "Quote":
                return '\"';

            case "Dollar":
                return '$';

            default:
                return '.';
        }
    }
}