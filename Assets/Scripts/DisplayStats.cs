using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Profiling;
using System;

public class DisplayStats : MonoBehaviour
{
	float deltaTime = 0.0f;
	private Text fpscount;
	private float playTimeStart;
	
	void Start()
	{
		fpscount = gameObject.GetComponent<Text>();
		playTimeStart = Time.time;
	}
	
	void Update(){
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
		float fps = 1.0f / deltaTime;
		float memoryUsage = Profiler.GetTotalAllocatedMemoryLong()/1e+6f;
		string text = string.Format("fps: {0:0.} \nmem: {1:0.} MB \ntime played:{2:0} S", fps, memoryUsage, Time.time-playTimeStart);
		fpscount.text = text;
	} 
}
