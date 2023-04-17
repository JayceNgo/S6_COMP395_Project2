using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Traffic
{
	public enum TrafficType { Pedestrian, Vehicle }

	public class TrafficSystem : MonoBehaviour 
	{
		// -------------------------------------------------------------------
		// Singleton
		private static TrafficSystem _Instance;
		public static TrafficSystem Instance
		{
			get
			{
				if(_Instance == null)
					_Instance = FindObjectOfType<TrafficSystem>();
				return _Instance;
			}
		}

		// -------------------------------------------------------------------
		// Properties

		public bool drawGizmos = false;
		public GameObject vehiclePrefab; // TODO - Get from object pool
		public Transform pool;	// TODO - Get from object pool
		public bool spawnOnStart = true;
		public int maxRoadVehicles = 100;
		public int maxTrains = 5;
		public int maxPedestrians = 100;

		// -------------------------------------------------------------------
		// Initialization

		private List<Road> m_Roads = new List<Road>();

		private void Start () 
		{
			Road[] roadsFound = FindObjectsOfType<Road>();
			foreach(Road r in roadsFound)
				m_Roads.Add(r);

			if(spawnOnStart)
			{
				for(int i = 0; i < maxRoadVehicles; i++)
					SpawnRoadVehicle(true);
			}
		}

		// -------------------------------------------------------------------
		// Update

		private void Update()
		{
			if(Input.GetKeyUp(KeyCode.Return))
				SpawnRoadVehicle(true);
		}

		// -------------------------------------------------------------------
		// Spawn

		private int m_RoadVehicleSpawnAttempts;

		private void SpawnRoadVehicle(bool reset)
		{
			if(reset)
				m_RoadVehicleSpawnAttempts = 0;
			int index = UnityEngine.Random.Range(0, m_Roads.Count);
			Road road = m_Roads[index];
			VehicleSpawn spawn;
			if(!road.TryGetVehicleSpawn(out spawn))
			{
				m_RoadVehicleSpawnAttempts++;
				if(m_RoadVehicleSpawnAttempts < m_Roads.Count)
					SpawnRoadVehicle(false);
				return;
			}
			Vehicle newVehicle = Instantiate(vehiclePrefab, spawn.spawn.position, spawn.spawn.rotation, pool.transform).GetComponent<Vehicle>();
			newVehicle.Initialize(road, spawn.destination);
		}

		// -------------------------------------------------------------------
		// Static

		public float GetAgentSpeedFromKPH(int kph)
		{
			return kph * .02f;
		}
	}
}

