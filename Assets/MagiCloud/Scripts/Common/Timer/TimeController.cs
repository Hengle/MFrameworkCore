﻿using MagiCloud.KGUI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace MagiCloud.Common
{
    /// <summary>
    /// 时间控制
    /// </summary>
    public class TimeController :SerializedMonoBehaviour
    {
        public float virtualTime = 10;      //虚拟时间
        public float realTime = 20;         //真实时间
        public KGUI_Toggle timeToggle;      //控制开关
        public Text showTimeText;           //显示文本

        private float time = 0;              //计时
        private int status = -1;             //-1表示未开始，1表示开始，0表示暂停   
        private Timer timer;

        public UnityEvent playEvent;
        public UnityEvent pauseEvent;
        public UnityEvent stopEvent;
        public UnityEvent<float> playingEvent;

        public string TimeString => (time).ToString("f1");
        public float Progress { get; private set; }

        public bool Playing => status==1;
        public bool Pausing => status==0;
        public bool Stopping => status==-1;

        private void Start()
        {
            timeToggle?.OnValueChanged.AddListener(OnChange);
        }

        #region 计时
        private void OnChange(bool play)
        {
            if (status==-1)
                Play();
            else if (status==0)
                Connitue();
            else
                Pause();
        }


        private void OnTime(float t)
        {
            Progress=t;
            time =t*virtualTime;
            if (showTimeText!=null)
                showTimeText.text=TimeString;
            playingEvent?.Invoke(t);
        }

        private void OnCompleted()
        {
            timeToggle.IsValue=false;
            stopEvent?.Invoke();
            status=-1;
        }

        public void Pause()
        {
            pauseEvent?.Invoke();
            timer.PauseTimer();
            status=0;
        }

        public void Connitue()
        {
            timer.ConnitueTimer();
            status=1;
        }

        public void Play()
        {
            if (timer==null)
            {
                timer  =new GameObject("timer").AddComponent<Timer>();
                timer.StartTiming(realTime,OnCompleted,OnTime);
            }
            else
                timer.ReStartTimer();
            playEvent?.Invoke();
            status =1;
        }

        #endregion
        private void OnDestroy()
        {

            timeToggle?.OnValueChanged.RemoveListener(OnChange);
            if (timer!=null)
                Destroy(timer);
        }
    }
}
