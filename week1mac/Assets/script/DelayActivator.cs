using UnityEngine;
using System.Collections;

public class DelayActivator : MonoBehaviour
{
    [Header("设置")]
    public GameObject targetObject;
    public float delayTime = 15f;
    public float fadeDuration = 2.0f;

    [Header("★ 外观控制")]
    [Range(0f, 1f)]
    public float targetAlpha = 0.3f; // 透明度 (0.3=玻璃, 1=实心)

    // 内部变量
    private Material beamMat;
    private Color baseColor;
    private Color emissionColor;
    private Light childLight;
    private float originalLightIntensity;

    // ★ 新增：用来存你原本的大小
    private Vector3 originalScale;

    public void BeginTimer()
    {
        if (targetObject == null) return;

        // 1. 先激活物体
        targetObject.SetActive(true);

        // 2. ★★★ 关键修改：立刻记住你现在的实际大小！★★★
        originalScale = targetObject.transform.localScale;

        // 3. 处理材质
        Renderer rend = targetObject.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            beamMat = rend.material;

            if (beamMat.HasProperty("_BaseColor")) baseColor = beamMat.GetColor("_BaseColor");
            else if (beamMat.HasProperty("_Color")) baseColor = beamMat.GetColor("_Color");

            if (beamMat.HasProperty("_EmissionColor")) emissionColor = beamMat.GetColor("_EmissionColor");
            else emissionColor = Color.black;

            SetMaterialLook(0f, Color.black);
        }

        // 4. 处理灯光
        childLight = targetObject.GetComponentInChildren<Light>();
        if (childLight != null)
        {
            originalLightIntensity = childLight.intensity;
            childLight.intensity = 0f;
        }

        // 5. 缩放归零 (隐身)
        targetObject.transform.localScale = Vector3.zero;

        // 开始
        StartCoroutine(FadeInRoutine());
    }

    IEnumerator FadeInRoutine()
    {
        Debug.Log("倒计时开始...");
        yield return new WaitForSeconds(delayTime);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            t = t * t * (3f - 2f * t);

            // ★★★ 变大：变回 originalScale (你原本的大小)，而不是 1
            targetObject.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);

            // 变亮
            if (beamMat != null)
            {
                float currentAlpha = Mathf.Lerp(0f, targetAlpha, t);
                Color currentEmission = Color.Lerp(Color.black, emissionColor, t);
                SetMaterialLook(currentAlpha, currentEmission);
            }

            // 开灯
            if (childLight != null)
            {
                childLight.intensity = Mathf.Lerp(0f, originalLightIntensity, t);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // 确保最后严丝合缝
        targetObject.transform.localScale = originalScale;
        SetMaterialLook(targetAlpha, emissionColor);
        if (childLight != null) childLight.intensity = originalLightIntensity;
    }

    void SetMaterialLook(float alpha, Color emission)
    {
        if (beamMat == null) return;

        Color temp = baseColor;
        temp.a = alpha;

        if (beamMat.HasProperty("_BaseColor")) beamMat.SetColor("_BaseColor", temp);
        else if (beamMat.HasProperty("_Color")) beamMat.SetColor("_Color", temp);

        if (beamMat.HasProperty("_EmissionColor")) beamMat.SetColor("_EmissionColor", emission);
    }
}