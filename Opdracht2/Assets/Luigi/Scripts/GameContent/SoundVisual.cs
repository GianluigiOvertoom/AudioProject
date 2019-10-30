
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundVisual : MonoBehaviour
{
    public float _rmsValue;
    public float _dbValue;
    public float _pitchValue;
    public float _maxVisualScale;
    public float _visualModifier = 50;
    public float _smoothSpeed = 10;
    public float _keepPercentage = 0.5f;
    public int _amountVisual = 32;

    [SerializeField] private GameObject[] _pin;

    private AudioSource _source;
    private float[] _samples;
    private float[] _spectrum;
    private float _sampleRate;
    private Transform[] _visualList;
    private float[] _visualScale;
    private ObjectPooler _objectpool;
    private const int SAMPLE_SIZE = 1024;

    private void Start()
    {
        _source = GetComponent<AudioSource>();
        _samples = new float[SAMPLE_SIZE];
        _spectrum = new float[SAMPLE_SIZE];
        _sampleRate = AudioSettings.outputSampleRate;
        
        _objectpool = ObjectPooler.Instance;
        SpawnCircle();
    }

    private void SpawnLine()
    {
        _visualScale = new float[_amountVisual];
        _visualList = new Transform[_amountVisual];

        for (int i = 0; i < _amountVisual; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            _visualList[i] = go.transform;
            _visualList[i].position = Vector3.right * i;
        }
    }
    private void SpawnCircle()
    {
        _visualScale = new float[_amountVisual];
        _visualList = new Transform[_amountVisual];

        Vector3 center = Vector3.zero;
        float radius = 10f;

        for (int i = 0; i < _amountVisual; i++)
        {
            float ang = i * 1f / _amountVisual;
            ang = ang * Mathf.PI * 2;

            float x = center.x + Mathf.Cos(ang) * radius;
            float y = center.y + Mathf.Sin(ang) * radius;

            Vector3 pos = center + new Vector3(x, y, 0);

            GameObject go = _pin[i] as GameObject;
            go.transform.position = pos;
            go.transform.rotation = Quaternion.LookRotation(Vector3.forward, pos);
            _visualList[i] = go.transform;
            
        }
    }

    private void Update()
    {
        AnalyzeSound();
        UpdateVisual();
    }

    private void AnalyzeSound()
    {
        _source.GetOutputData(_samples, 0);
        int i = 0;
        float sum = 0;
        for (; i < SAMPLE_SIZE; i++)
        {
            sum = _samples[i] * _samples[i];
        }
        _rmsValue = Mathf.Sqrt(sum / SAMPLE_SIZE);
        _dbValue = 20 * Mathf.Log10(_rmsValue / 0.1f);
        _source.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
        float maxV = 0;
        var maxN = 0;
        for (i = 0; i < SAMPLE_SIZE; i++)
        {
            if (!(_spectrum[i] > maxV) || !(_spectrum[i] > 0))
            {
                continue;
            }

            maxV = _spectrum[i];
            maxN = i;
        }

        float freqN = maxN;
        if (maxN > 0 && maxN < SAMPLE_SIZE - 1)
        {
            var dL = _spectrum[maxN - 1] / _spectrum[maxN];
            var dR = _spectrum[maxN + 1] / _spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        _pitchValue = freqN * (_sampleRate / 2) / SAMPLE_SIZE;
    }

    private void UpdateVisual()
    {
        int visualIndex = 0;
        int spectrumIndex = 0;
        int averageSize = (int)((SAMPLE_SIZE * _keepPercentage) / _amountVisual);

        while (visualIndex < _amountVisual)
        {
            int j = 0;
            float sum = 0;
            while (j < averageSize)
            {
                sum += _spectrum[spectrumIndex];
                spectrumIndex++;
                j++;
            }

            float scaleY = sum / averageSize * _visualModifier;
            _visualScale[visualIndex] -= Time.deltaTime * _smoothSpeed;

            if (_visualScale[visualIndex] < scaleY)
            {
                _visualScale[visualIndex] = scaleY;
            }

            if (_visualScale[visualIndex] > _maxVisualScale)
            {
                _visualScale[visualIndex] = _maxVisualScale;
            }

            _visualList[visualIndex].localScale = Vector3.one + Vector3.up * _visualScale[visualIndex];
            visualIndex++;
        }
    }
}
