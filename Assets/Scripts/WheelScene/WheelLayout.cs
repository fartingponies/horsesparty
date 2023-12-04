using UnityEngine;

using static Common;
using static SceneFader;

public class WheelLayout : MonoBehaviour
{
    public RectTransform title;
    public RectTransform wheelSetup;
    public RectTransform currentRound;

    void Start()
    {
        float currentRoundScale = Mathf.Min(safeUpperRight.x - safeLowerLeft.x, safeUpperRight.y - safeLowerLeft.y) * 0.25f;
        currentRound.localScale = new Vector2(currentRoundScale, currentRoundScale);
        currentRound.position = new Vector2(safeLowerLeft.x * 1f + currentRound.rect.width * currentRoundScale * 0.5f, safeUpperRight.y - currentRound.rect.height * currentRoundScale * 0.5f);

        float titleScale = (Mathf.Min(safeUpperRight.x - safeLowerLeft.x, safeUpperRight.y - safeLowerLeft.y)) * 0.85f / title.rect.width;
        title.localScale = new Vector2(titleScale, titleScale);

        title.position = new Vector2(0f, safeUpperRight.y * 0.8f - title.rect.height * titleScale * 0.5f);

        float widthMaxScale = (safeUpperRight.x - safeLowerLeft.x) * 0.9f / wheelSetup.rect.width;
        float heightMaxScale = (safeUpperRight.y - safeLowerLeft.y) * 0.5f / wheelSetup.rect.height;

        float wheelScale = Mathf.Min(widthMaxScale, heightMaxScale);
        wheelSetup.localScale = new Vector2(wheelScale, wheelScale);

        wheelSetup.position = new Vector2(0f, safeLowerLeft.y + (safeUpperRight.y - safeLowerLeft.y - title.rect.height * titleScale) * 0.5f);
    }
}
