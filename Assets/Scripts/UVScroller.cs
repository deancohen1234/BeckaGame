using UnityEngine;

[ExecuteInEditMode]
public class UVScroller : MonoBehaviour
{

    private float m_fFlowMapOffset0 = 0.0f;
    private float m_fFlowMapOffset1 = 0.0f;
    public float m_fFlowSpeed = 0.05f;
    public float m_fCycle = 0.15f;
    public float m_fWaveMapScale = 2.0f;

    public void Update()
    {
        //update the flow map offsets for both layers
        m_fFlowMapOffset0 += m_fFlowSpeed * Time.deltaTime;
        m_fFlowMapOffset1 += m_fFlowSpeed * Time.deltaTime;

        if (m_fFlowMapOffset0 >= m_fCycle)
            m_fFlowMapOffset0 = 0.0f;

        if (m_fFlowMapOffset1 >= m_fCycle)
            m_fFlowMapOffset1 = 0.0f;

        float _fHalfCycle = m_fCycle * 0.5f;

        Shader.SetGlobalFloat("flowMapOffset0", m_fFlowMapOffset0);
        Shader.SetGlobalFloat("flowMapOffset1", m_fFlowMapOffset1);
        Shader.SetGlobalFloat("halfCycle", _fHalfCycle);
        Shader.SetGlobalFloat("fWaveSpeed", m_fFlowSpeed);
        Shader.SetGlobalFloat("_WaveScale", m_fWaveMapScale);
    }
}
