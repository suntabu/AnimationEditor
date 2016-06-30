using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Inlycat;
using UnityEditor;

public class AnimationEditorPanel : EditorWindow
{

    [MenuItem("勤享/动画编辑器")]
    static void AddWindow()
    {
        //创建窗口
        Rect wr = new Rect(0, 0, 500, 500);
        AnimationEditorPanel window = (AnimationEditorPanel)EditorWindow.GetWindow(typeof(AnimationEditorPanel), true, "动画编辑器");
        window.Show();
    }

    private List<AnimationModel> _animations;

    private AnimationModel _currentEditAnimation;

    private static string _configFileName = "animationData.json";

    private string _animationConfigPath = Application.streamingAssetsPath + "/" + _configFileName;

    private Vector2 _animationScroll;
    private Vector2 _effectScroll;
    public void Awake()
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


    void OnGUI()
    {
        _animationScroll = EditorGUILayout.BeginScrollView(_animationScroll, GUILayout.Height(100));
        for (int i = 0; i < 2000; i++)
        {
            GUILayout.Space(5);
            AnimationModel am = _animations[0];
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(am.animationName);
            if (GUILayout.Button("编辑", GUILayout.Width(150)))
            {
                Debug.Log("开始编辑：" + am.animationName);

                if (_currentEditAnimation != null)
                {
                    Save();
                }
                _currentEditAnimation = am;
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
        }
        EditorGUILayout.EndScrollView();


        // 动画详情
        GUILayout.Space(15);

        if (_currentEditAnimation != null)
        {
            _currentEditAnimation.animationName = LayoutStringProperty("动画名称", _currentEditAnimation.animationName);
            _currentEditAnimation.duration = LayoutFloatProperty("持续时间", _currentEditAnimation.duration);
            GUILayout.Space(5);
            GUILayout.Label("子动画");
            _effectScroll = EditorGUILayout.BeginScrollView(_effectScroll);
            for (int i = 0; i < _currentEditAnimation.elements.Count; i++)
            {
                var effect = _currentEditAnimation.elements[i];
                LayoutEffectModel(effect);

                GUILayout.Space(15);
            }
            EditorGUILayout.EndScrollView();
        }

        GUILayout.Space(15);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("run"))
        {
            RunAnimation();
        }

        if (GUILayout.Button("save"))
        {
            Save();
        }
    }


    private void LayoutEffectModel(ElementModel element)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("素材路径" + "：");
        element.elementName = EditorGUILayout.TextField(element.elementName, GUILayout.Height(30));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("开始时间" + "：");
        element.startTime = EditorGUILayout.FloatField(element.startTime, GUILayout.Height(30));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("结束时间" + "：");
        element.stopTime = EditorGUILayout.FloatField(element.stopTime, GUILayout.Height(30));
        GUILayout.EndHorizontal();

        LayoutValues("路径变化：", element.pathValues);
        LayoutValues("缩放变化：", element.scaleValues);
        //layoutValues("旋转变化：", element.rotateValues);
    }

    private static void LayoutValues(string title, float[] values)
    {
        try
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title + "：");
            GUILayout.BeginVertical();
            for (int i = 0; i < values.Length; i++)
            {
                if (i % 3 == 0)
                {
                    var x = values[i];
                    var y = values[i + 1];
                    var t = values[i + 2];

                    Vector3 result = EditorGUILayout.Vector3Field(i / 3 + " :", new Vector3(x, y, t));
                    values[i] = result.x;
                    values[i + 1] = result.y;
                    values[i + 2] = result.z;
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            values = new float[] { };
        }

    }

    private float collapsedTime = 0;
    private void RunAnimation()
    {
        collapsedTime = 0;
        CoroutineManager.Instance.DoAction(CoroutineRunner.EveryFrameDoOneAction(delegate
        {
            collapsedTime += Time.deltaTime;
            if (collapsedTime > _currentEditAnimation.duration)
            {
                collapsedTime = 0;
                Debug.Log("done!");
                return true;
            }
            else
            {
                return false;
            }
        }));
    }

    private void Save()
    {

    }

    private string LayoutStringProperty(string title, string value)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(title + "：");
        string result = GUILayout.TextArea(value, GUILayout.Height(30));
        GUILayout.EndHorizontal();
        return result;
    }

    private float LayoutFloatProperty(string title, float value)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(title + "：");
        float result = EditorGUILayout.FloatField(value, GUILayout.Height(30));
        GUILayout.EndHorizontal();
        return result;
    }
}
