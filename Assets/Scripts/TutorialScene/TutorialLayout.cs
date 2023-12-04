using UnityEngine;

using static Common;

public class TutorialLayout : MonoBehaviour
{
    public RectTransform title;
    public RectTransform skipButton;
    public RectTransform tutorialSetup;
    public RectTransform tutorialLabel;

    void Start()
    {
        float tutorialLabelScale = (Mathf.Min(safeUpperRight.x - safeLowerLeft.x, safeUpperRight.y - safeLowerLeft.y)) / title.rect.width;
        tutorialLabel.localScale = new Vector2(tutorialLabelScale, tutorialLabelScale);
        tutorialLabel.position = new Vector2(safeLowerLeft.x * 1f + tutorialLabel.rect.width * tutorialLabelScale * 0.5f, safeUpperRight.y - tutorialLabel.rect.height * tutorialLabelScale * 0.5f);
        
        skipButton.position = new Vector2(safeUpperRight.x * 1.0f - skipButton.rect.width * skipButton.lossyScale.x * 0.5f, safeUpperRight.y - skipButton.rect.height * skipButton.lossyScale.y * 0.5f);

        float titleScale = (Mathf.Min(safeUpperRight.x - safeLowerLeft.x, safeUpperRight.y - safeLowerLeft.y)) * 0.75f / title.rect.width;
        title.localScale = new Vector2(titleScale, titleScale);
        title.position = new Vector2(0f, safeUpperRight.y * 0.87f - title.rect.height * titleScale * 0.5f);

        float widthMaxScale = (safeUpperRight.x - safeLowerLeft.x) * 0.9f / tutorialSetup.rect.width;
        float heightMaxScale = (safeUpperRight.y - safeLowerLeft.y) * 0.45f / tutorialSetup.rect.height;

        float wheelScale = Mathf.Min(widthMaxScale, heightMaxScale);
        tutorialSetup.localScale = new Vector2(wheelScale, wheelScale);

        tutorialSetup.position = new Vector2(0f, safeLowerLeft.y + (safeUpperRight.x - safeLowerLeft.x) * 0.05f + tutorialSetup.rect.height * wheelScale * 0.5f);
    }
}
