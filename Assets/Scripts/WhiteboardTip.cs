using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WhiteboardTip : MonoBehaviour
{
    [SerializeField] private Transform _tip;
    [SerializeField, Range(1, 10)] private int _penSize = 5;
    [SerializeField] private TipType tipType = TipType.Cube;
    private Renderer _renderer;
    private Color[] _colors;
    private float _tipLength;
    private RaycastHit _touch;
    private Whiteboard _whiteboard;
    private Vector2 _touchPos;
    private bool _touchedLastFrame;
    private Vector2 _lastTouchPos;
    private Quaternion _lastTouchRot;

    private void Start()
    {
        _renderer = _tip.GetComponent<Renderer>();
        _colors = Enumerable.Repeat(_renderer.material.color, _penSize * _penSize).ToArray();
        switch (tipType)
        {
            case TipType.Sphere:
                _tipLength = _tip.localScale.z / 2;
                break;
            case TipType.Cube:
                _tipLength = _tip.localScale.z * Mathf.Sqrt(3) / 2;
                break;
            default:
                _tipLength = 0.1f;
                break;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {

    }
    private void OnCollisionStay(Collision collision)
    {
        Draw();
        collision.GetContact(0);
    }
    private void OnCollisionExit(Collision collision)
    {

    }
    //private void Update()
    //{
    //    Draw();
    //}

    private void Draw()
    {
        if (Physics.Raycast(_tip.position, transform.up, out _touch, _tipLength))
        {
            if (_touch.transform.CompareTag("Whiteboard"))
            {
                if (_whiteboard == null)
                {
                    _whiteboard = _touch.transform.GetComponent<Whiteboard>();
                }
                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);
                int x = (int)(_touchPos.x * _whiteboard._textureSize.x - (_penSize / 2));
                int y = (int)(_touchPos.y * _whiteboard._textureSize.y - (_penSize / 2));
                if (y < 0 || y > _whiteboard._textureSize.y || x < 0 || x > _whiteboard._textureSize.x) return;
                if (_touchedLastFrame)
                {
                    _whiteboard._texture.SetPixels(x, y, _penSize, _penSize, _colors);
                    for (float f = 0.01f; f < 1.00f; f += 0.01f)
                    {
                        int lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        int lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                        _whiteboard._texture.SetPixels(lerpX, lerpY, _penSize, _penSize, _colors);
                    }
                    transform.rotation = _lastTouchRot;
                    _whiteboard._texture.Apply();
                }
                _lastTouchPos = new Vector2(x, y);
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;
                return;
            }
        }
        _whiteboard = null;
        _touchedLastFrame = false;
    }
}
