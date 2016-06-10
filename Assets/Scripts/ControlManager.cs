﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Inlycat;
using UnityEngine.Events;
using UnityEngine.UI;

public class ControlManager : MonoBehaviour
{
    public ScrollRect ScrollView;
    public Button RunAnimationBtn;
    public RawImage ImagePrefab;
    public RectTransform effectRoot;

    private List<AnimationModel> _animations;
    private AnimationModel _currentAnimation;

    private static string _configFileName = "animationData.json";
    private static string _configBasePath = @"E:\_Code\UnityProjects\AnimationEditor\Assets\StreamingAssets";

    private static string _animationConfigPath = _configBasePath + "/" + _configFileName;

    void Awake()
    {
        if (File.Exists(_animationConfigPath))
        {
            string content = File.ReadAllText(_animationConfigPath);
            _animations = Utils.getJsonArray<AnimationModel>(content);
        }

        if (_animations == null)
        {
            _animations = new List<AnimationModel>();
        }

        Debug.Log(_animations.Count);
    }


    void Start()
    {
        for (int i = 0; i < 200; i++)
        {
            AnimationModel am = _animations[0];

            Button button = Instantiate(Resources.Load<GameObject>("Prefabs/Button")).GetComponent<Button>();
            button.transform.SetParent(ScrollView.content);
            button.GetComponentInChildren<Text>().text = am.animationName;
            button.onClick.AddListener(delegate { StartEditAnimation(am); });

        }

        RunAnimationBtn.onClick.AddListener(RunAnimation);
    }

    void Update()
    {

    }

    void StartEditAnimation(AnimationModel animation)
    {
        Debug.Log("开始编辑：" + animation.animationName);

        if (_currentAnimation != null)
        {
            Save();
        }
        _currentAnimation = animation;
    }













    private float collapsedTime = 0;
    private void RunAnimation()
    {
        collapsedTime = 0;
        CoroutineManager.Instance.DoAction(CoroutineRunner.EveryFrameDoOneAction(delegate
        {
            if (!_currentAnimation.IsInit)
            {
                SpawnAnimation(_currentAnimation);

                return true;
            }
            else
            {
                collapsedTime += Time.deltaTime;
                for (int i = 0; i < _currentAnimation.effects.Count; i++)
                {
                    var effect = _currentAnimation.effects[i];
                    var v = effect.PathFitting.GetValue(collapsedTime) - coordnateOffeset;
                    effect.ImageGameObject.rectTransform.anchoredPosition = new Vector2(v.x, -v.y);
                    effect.ImageGameObject.rectTransform.localScale = effect.ScaleFitting.GetValue(collapsedTime);
                    Debug.Log(effect.ImageGameObject.rectTransform.localScale + "    " + effect.ScaleFitting.GetValue(collapsedTime));
                }
                if (collapsedTime > _currentAnimation.duration)
                {
                    collapsedTime = 0;
                    Debug.Log("done!");
                    return false;
                }
                else
                {
                    return true;
                }

            }


        }));
    }

    private void Save()
    {

    }


    private void SpawnAnimation(AnimationModel animation)
    {
        container_width = effectRoot.rect.width;
        container_height = effectRoot.rect.height;

        for (int i = 0; i < animation.effects.Count; i++)
        {
            var effect = animation.effects[i];

            RawImage image = GameObject.Instantiate(ImagePrefab);
            var tex = new Texture2D(0, 0);
            tex.LoadImage(File.ReadAllBytes(Path.Combine(_configBasePath, "anim/" + effect.effectName)));
            image.texture = tex;
            image.transform.SetParent(effectRoot);
            image.SetNativeSize();
            image.gameObject.SetActive(true);

            var hwRatio = image.rectTransform.sizeDelta.y / image.rectTransform.sizeDelta.x;

            image.rectTransform.sizeDelta = new Vector2(effect.size[0] * container_width, effect.size[1] * container_width * hwRatio);
            var v = new Vector2(effect.position[0] * container_width,
                effect.position[1] * container_height) - coordnateOffeset;
            image.rectTransform.anchoredPosition = new Vector2(v.x, -v.y);

            effect.ImageGameObject = image;
        }

        animation.IsInit = true;
    }

    public static float container_width = 750;
    public static float container_height = 1334;

    public float half_container_width
    {
        get { return container_width * 0.5f; }
    }

    public float half_container_height
    {
        get
        {
            return container_height * 0.5f;
        }
    }

    public Vector2 coordnateOffeset
    {
        get { return new Vector2(half_container_width, half_container_height); }
    }
}