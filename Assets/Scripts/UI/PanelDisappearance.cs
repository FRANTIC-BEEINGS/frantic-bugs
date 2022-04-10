using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelDisappearance : MonoBehaviour
{
    [SerializeField] private AnimationCurve LifeCurve;
    private const float DEATHTIME = 2; // second
    private float _lifeTime;
    private Image _image;
    private Color _color;

    void Start()
    {
        _lifeTime = 0;
        _image = gameObject.GetComponent<Image>();
        _color = _image.color;
    }

    void Update()
    {
        if (_lifeTime < DEATHTIME)
        {
            _color.a = LifeCurve.Evaluate(_lifeTime/DEATHTIME);
            _image.color = _color;
            _lifeTime += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
