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
    public float TimeBetweenCharacters;

    //public int CharactersPerLine;
    public TextDisposition Disposition;
    [Header("Sound")]
    public AudioSource SoundOnCharacter;
    public bool SoundEnabled;    
    [Header("Spin Effect")]
    public int AmountOfCycles;
    public int AnglesPerFrame;

    private Sprite[] fontSpritesArray;
    private Dictionary<char, Sprite> fontSprites;
    private bool _initialized;
    private string innerText;
    private int charactersPerLine;
    private RectTransform rectTransform;

    public bool IsFullyRendered { get; private set; }

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

        if (!_initialized)
        {
            SetText();
        }
        this.innerText = Text;
    }

    private void Update()
    {
        if (innerText != Text)
        {
            IsFullyRendered = false;
            innerText = Text;
            SetText();
        }
    }

    private void OnDisable()
    {
        this.Text = "";
        SetText();
    }

    public void SetColor(Color color)
    {
        this.TextureColor = color;
        SetText();
    }

    private void SetText()
    {
        if (!this.isActiveAndEnabled)
        {
            return;
        }

        if (DialogueArrow != null) DialogueArrow.IsBlinking = false;
        StopAllCoroutines();        

        _initialized = true;
        foreach (Transform glyph in this.transform)
        {
            Destroy(glyph.gameObject);
        }

        var charList = (InvertText ? this.Text.Reverse() : this.Text).ToArray();

        switch (this.Disposition)
        {
            case TextDisposition.SingleLine:
                StartCoroutine(SetTextSingleLine(charList));
                break;

            case TextDisposition.MultiLine:
                StartCoroutine(SetTextMultiLine(charList));
                break;

            case TextDisposition.MultiLineHyphenated:
                StartCoroutine(SetTextMultiLineHyphenated(charList));
                break;
        }
    }

    private IEnumerator FillTextSingleLine(char[] text, int lineOffset = 0)
    {
        int soundplay = 0;
        Vector2 offset = Vector2.zero;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (c == ' ')
            {
                continue;
            }

            SpriteRenderer renderer = CreateGlyph(c);
            offset.x = (i * renderer.sprite.rect.size.x + this.CharDistance*i) * (this.Scale.x > 0 ? this.Scale.x : 1) * (RightToLeft ? -1 : 1);
            offset.y = lineOffset * -renderer.sprite.rect.size.y * (this.Scale.x > 0 ? this.Scale.x : 1);
            renderer.transform.localPosition = new Vector3(this.Offset.x + offset.x - this.rectTransform.sizeDelta.x/2 + renderer.sprite.rect.size.x/2,
                                                           this.Offset.y + offset.y + this.rectTransform.sizeDelta.y / 2 - renderer.sprite.rect.size.y/2, 0);

            if (SoundEnabled && SoundOnCharacter != null && soundplay%2 ==0)
            {
                SoundOnCharacter.pitch = 0.9f + (Random.value / 6f);
                SoundOnCharacter.Play();
            }
            soundplay++;
            yield return new WaitForSeconds(TimeBetweenCharacters);
        }        
    }

    private IEnumerator SetTextSingleLine(char[] text)
    {
        yield return FillTextSingleLine(text);
        if (DialogueArrow != null) DialogueArrow.IsBlinking = true;
        IsFullyRendered = true;
    }

    private IEnumerator SetTextMultiLine(char[] text)
    {
        char[][] lines = text.GroupByWords(this.charactersPerLine).Select(h => h.ToArray()).ToArray();
        int lineNumber = 0;
        foreach (char[] line in lines)
        {
            yield return FillTextSingleLine(line, lineNumber);
            lineNumber++;
        }
        if (DialogueArrow != null) DialogueArrow.IsBlinking = true;
        IsFullyRendered = true;
    }

    private IEnumerator SetTextMultiLineHyphenated(char[] text)
    {
        char[][] lines = text.HyphenateAllLines(this.charactersPerLine).Select(h => h.ToArray()).ToArray();
        int lineNumber = 0;
        foreach (char[] line in lines)
        {
            yield return FillTextSingleLine(line, lineNumber);
            lineNumber++;
        }
        if (DialogueArrow != null) DialogueArrow.IsBlinking = true;
        IsFullyRendered = true;
    }

    private SpriteRenderer CreateGlyph(char c)
    {
        GameObject glyph = new GameObject("Glyph_" + c.ToString());
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