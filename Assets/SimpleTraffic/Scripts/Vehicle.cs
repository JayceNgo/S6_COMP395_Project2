using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Traffic
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(NavMeshAgent))]
	public class Vehicle : Agent 
	{
		private NavSection m_CurrentNavSection;
		private NavConnection m_CurrentOutConnection;

		// -------------------------------------------------------------------
		// Properties

		[Header("Vehicle")]
		public Transform front;

		// -------------------------------------------------------------------
		// Initialization

		public virtual void Initialize(NavSection navSection, NavConnection destination)
		{
			m_CurrentNavSection = navSection;
            RegisterVehicle(m_CurrentNavSection, true);
			m_CurrentOutConnection = destination;
			agent.enabled = true;
			speed = TrafficSystem.Instance.GetAgentSpeedFromKPH(Mathf.Min(navSection.speedLimit, maxSpeed));
			agent.speed = speed;
			agent.destination = destination.transform.position;
		}

        public virtual void RegisterVehicle(NavSection section, bool isAdd)
        {
            section.RegisterVehicle(this, isAdd);
        }

		// -------------------------------------------------------------------
		// Update

		public override void Update()
		{
			if(agent.isOnNavMesh)
			{
				m_Blocked = CheckBlocked();
			}
			base.Update();
		}

		public override bool CheckStop()
		{
			if(m_Blocked || isWaiting)
				return true;
			return false;
		}

		// -------------------------------------------------------------------
		// Collisions

		public override void OnTriggerEnter(Collider col)
		{
			if(col.tag == "RoadConnection")
			{
				NavConnection connection = col.GetComponent<NavConnection>();
				if(connection.navSection != m_CurrentNavSection)
					SwitchRoad(connection);
			}
			base.OnTriggerEnter(col);
		}

		// -------------------------------------------------------------------
		// Blocked

		private bool m_Blocked;

		private float m_BlockedDistance = .25f;
		public float blockedDistance
		{
			get { return m_BlockedDistance; }
			set { m_BlockedDistance = value; }
		}

		private bool CheckBlocked()
		{
			Vector3 forward = transform.TransformDirection(Vector3.forward);
			RaycastHit hit;
			if (Physics.Raycast(front.position, forward, out hit))
			{
				if(Vector3.Distance(front.position, hit.point) < m_BlockedDistance)
				{
					if(hit.transform.tag == "Gib" || hit.transform.tag == "Unit")
						return true;
				}
				return false;
			}
			return false;
		}

		// -------------------------------------------------------------------
		// Switch Road

		private void SwitchRoad(NavConnection newConnection)
		{
			RegisterVehicle(m_CurrentNavSection, false);
			speed = TrafficSystem.Instance.GetAgentSpeedFromKPH(Mathf.Min(newConnection.navSection.speedLimit, maxSpeed));
			agent.speed = speed;
			m_CurrentNavSection = newConnection.navSection;
			RegisterVehicle(m_CurrentNavSection, true);
			m_CurrentOutConnection = newConnection.GetOutConnection();
			if(m_CurrentOutConnection != null)
				agent.destination = m_CurrentOutConnection.transform.position;
		}
	}
}

