using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Traffic
{
	public class Junction : Road 
	{	
		// -------------------------------------------------------------------
		// Enum

		public enum PhaseType { Timed, OnDemand }

		// -------------------------------------------------------------------
		// Properties

		[Header("Junction")]
		public PhaseType type = PhaseType.Timed;
		public Phase[] phases;
		public JunctionTrigger[] triggers;
		public float phaseInterval = 5;

		// -------------------------------------------------------------------
		// Initialization

		public override void Start()
		{
			base.Start();
			if(phases.Length > 0)
				phases[0].Enable();
			foreach(JunctionTrigger jt in triggers)
				jt.junction = this;
		}

		// -------------------------------------------------------------------
		// Update

		private void Update()
		{
			if(type == PhaseType.Timed)
			{
				m_PhaseTimer += Time.deltaTime;
				if(!m_PhaseEnded && m_PhaseTimer > (phaseInterval * 0.5f))
					EndPhase();
				if(m_PhaseTimer > phaseInterval)
					ChangePhase();
			}
		}

		// -------------------------------------------------------------------
		// Phase

		float m_PhaseTimer;
		bool m_PhaseEnded;
		private int m_CurrentPhase;

		private void EndPhase()
		{
			m_PhaseEnded = true;
			phases[m_CurrentPhase].End();
		}

		public void ChangePhase()
		{
			m_PhaseTimer = 0;
			m_PhaseEnded = false;
			if(m_CurrentPhase < phases.Length - 1)
				m_CurrentPhase++;
			else
				m_CurrentPhase = 0;
			phases[m_CurrentPhase].Enable();
		}

        public void TryChangePhase()
        {
            if (!HasActiveTrains())
                ChangePhase();
        }

		// -------------------------------------------------------------------
		// Data Classes

		[Serializable]
		public class Phase
		{
			public WaitZone[] positiveZones;
			public WaitZone[] negativeZones;
			public TrafficLight[] positiveLights;
			public TrafficLight[] negativeLights;

			public void Enable()
			{
				foreach(WaitZone zone in positiveZones)
					zone.canPass = true;
				foreach(TrafficLight light in positiveLights)
					light.SetLight(true);
				foreach(WaitZone zone in negativeZones)
					zone.canPass = false;
				foreach(TrafficLight light in negativeLights)
					light.SetLight(false);
			}

			public void End()
			{
				foreach(WaitZone zone in positiveZones)
					zone.canPass = false;
			}
		}
	}
}
