using UnityEngine;
using UnityEngine.UI;

internal class FPS : MainComponent<FPS>, IInitializeMain
{
    [SerializeField]
    private Text fpsText;

    public void Init<T>(T args)
    {
        SubscribeToUpdate(Updating);
    }

    private void Updating()
    {
        if (Time.deltaTime > 0)
        {
            fpsText.text = "FPS: " + ((int)(1 / Time.deltaTime)).ToString();
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            fpsText.gameObject.SetActive(!fpsText.gameObject.activeSelf);
        }
    }
}