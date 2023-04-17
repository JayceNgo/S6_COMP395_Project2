﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Traffic
{
	public class TrafficLight : MonoBehaviour 
	{
		public MeshRenderer Renderer;

		public void SetLight(bool input)
		{
			Renderer.material.SetColor("_Color", input ? Color.green : Color.red );
		}
	}
}
