using System;
using System.Collections.Generic;
using Enums;
using TMPro;
using UI.Common;
using UI.UIManager;
using UniRx;
using UnityEngine;

namespace UI.Windows
{
 public class DialogWindow : Window
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private int _offset;
        [SerializeField] private UIButton _continue;
        [SerializeField] private GameObject _nextIcon;
        [SerializeField] private TMP_Text _label;
        private event Action Next;

        private List<Anchors> _anchorsList = new List<Anchors>();
        protected override void Awake()
        {
            InitAnchors();
            base.Awake();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            _continue.OnClickAsObservable.Subscribe(_ => InContinue()).AddTo(ActivateDisposables);
        }

        private void InContinue()
        {
            Next?.Invoke();
        }
        public void ShowDialog(string text, Action action = null, AnchorsEnum type = AnchorsEnum.Center)
        {
            Next = action;
            _nextIcon.gameObject.SetActive(action != null);
            ShowDialog(_anchorsList.Find(model => model.Type == type));
            _label.text = text;
        }

        private void ShowDialog(Anchors anchor)
        {
            var rect = _content.rect;
            _content.anchorMin = anchor.Min;
            _content.anchorMax = anchor.Max;
            switch (anchor.Type)
            {
                case AnchorsEnum.LeftUp:
                    _content.anchoredPosition = new Vector2((rect.width / 2) + _offset, -1 * (rect.height / 2) - _offset );
                    break;
                case AnchorsEnum.Left:
                    _content.anchoredPosition = new Vector2((rect.width / 2) + _offset, 0 );
                    break;
                case AnchorsEnum.LeftDown:
                    _content.anchoredPosition = new Vector2((rect.width / 2) + _offset, (rect.height / 2) + _offset );
                    break;
                case AnchorsEnum.CenterUp:
                    _content.anchoredPosition = new Vector2(0, -1 * (rect.height / 2) - _offset );
                    break;
                case AnchorsEnum.Center:
                    _content.anchoredPosition = new Vector2(0, 0 );
                    break;
                case AnchorsEnum.CenterDown:
                    _content.anchoredPosition = new Vector2(0, (rect.height / 2) + _offset );
                    break;
                case AnchorsEnum.RightUp:
                    _content.anchoredPosition = new Vector2(-1 *(rect.width / 2) -  _offset, -1 * (rect.height / 2) - _offset );
                    break;
                case AnchorsEnum.Right:
                    _content.anchoredPosition = new Vector2(-1 *(rect.width / 2) -  _offset, 0 );
                    break;
                case AnchorsEnum.RightDown:
                    _content.anchoredPosition = new Vector2(-1 *(rect.width / 2) -  _offset, (rect.height / 2) + _offset );
                    break;
            }
        }

        private void InitAnchors()
        {
            _anchorsList.Add(new Anchors()
            {
                Type = AnchorsEnum.LeftUp,
                Min = new Vector2(0,1),
                Max = new Vector2(0,1)
            });
            
            _anchorsList.Add(new Anchors()
            {
                Type = AnchorsEnum.Left,
                Min = new Vector2(0,0.5f),
                Max = new Vector2(0,0.5f)
            });
            
            _anchorsList.Add(new Anchors()
            {
                Type = AnchorsEnum.LeftDown,
                Min = new Vector2(0,0),
                Max = new Vector2(0,0)
            });
            
            _anchorsList.Add(new Anchors()
            {
                Type = AnchorsEnum.CenterUp,
                Min = new Vector2(0.5f,1),
                Max = new Vector2(0.5f,1)
            });
            
            _anchorsList.Add(new Anchors()
            {
                Type = AnchorsEnum.Center,
                Min = new Vector2(0.5f,0.5f),
                Max = new Vector2(0.5f,0.5f)
            });
            
            _anchorsList.Add(new Anchors()
            {
                Type = AnchorsEnum.CenterDown,
                Min = new Vector2(0.5f,0),
                Max = new Vector2(0.5f,0)
            });
            
            _anchorsList.Add(new Anchors()
            {
                Type = AnchorsEnum.RightUp,
                Min = new Vector2(1,1),
                Max = new Vector2(1,1)
            });
            
            _anchorsList.Add(new Anchors()
            {
                Type = AnchorsEnum.Right,
                Min = new Vector2(1,0.5f),
                Max = new Vector2(1,0.5f)
            });
            
            _anchorsList.Add(new Anchors()
            {
                Type = AnchorsEnum.RightDown,
                Min = new Vector2(1,0),
                Max = new Vector2(1,0)
            });
        }
        [Serializable]
        private struct Anchors
        {
            public AnchorsEnum Type;
            public Vector2 Min;
            public Vector2 Max;
        }
    }
}