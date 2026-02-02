using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public static class Utilities
{
	/// <summary>
	/// Uses <see cref="Physics.OverlapSphere(Vector3, float, int)"/> to get game objects in range
	/// </summary>
	/// <param name="position"></param>
	/// <param name="range"></param>
	/// <param name="layerMask"></param>
	/// <param name="sortByDistance"></param>
	/// <param name="useRigidbody">When true, the game object on the attached rigidbody will be returned (and objects without one will be ignored).</param>
	/// <returns></returns>
	public static List<GameObject> GetGameObjectsInRange(Vector3 position, float range, LayerMask layerMask, bool sortByDistance = false, bool useRigidbody = true)
	{
		var collidersInRange = Physics.OverlapSphere(position, range, layerMask);
		var gameObjetsInRange = new List<GameObject>();

		foreach (Collider collider in collidersInRange)
		{
			if (useRigidbody)
			{
				if (collider.attachedRigidbody != null)
					gameObjetsInRange.Add(collider.attachedRigidbody.gameObject);
			}
			else
			{
				gameObjetsInRange.Add(collider.gameObject);
			}
		}

		if (sortByDistance)
			gameObjetsInRange.Sort((a, b) => Vector3.SqrMagnitude(a.transform.position - position).CompareTo(Vector3.SqrMagnitude(b.transform.position - position)));


        return gameObjetsInRange;
	}

    /// <summary>
    /// Uses <see cref="Physics.OverlapSphereNonAlloc(Vector3, float, Collider[], int)"/> to get game objects in range.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="range"></param>
    /// <param name="overlapResults"></param>
    /// <param name="layerMask"></param>
    /// <param name="gameObjectsInRange"></param>
    /// <param name="sortByDistance"></param>
    /// <param name="useRigidbody"></param>
    /// <returns></returns>
    public static List<GameObject> GetGameObjectsInRangeNonAlloc(Vector3 position, float range, ref Collider[] overlapResults, LayerMask layerMask, ref List<GameObject> gameObjectsInRange, bool sortByDistance = false, bool useRigidbody = true)
    {
        int collidersInRange = Physics.OverlapSphereNonAlloc(position, range, overlapResults, layerMask);
        for (int i = 0; i < collidersInRange; i++)
        {
            Collider collider = overlapResults[i];
            if (useRigidbody)
            {
                if (collider.attachedRigidbody != null)
                    gameObjectsInRange.Add(collider.attachedRigidbody.gameObject);
            }
            else
            {
                gameObjectsInRange.Add(collider.gameObject);
            }
        }

        if (sortByDistance)
            gameObjectsInRange.Sort((a, b) => Vector3.SqrMagnitude(a.transform.position - position).CompareTo(Vector3.SqrMagnitude(b.transform.position - position)));

        return gameObjectsInRange;
    }

    /// <summary>
    /// Uses <see cref="Physics.OverlapSphere(Vector3, float, int)"/> to get objects in range, then looks for <typeparamref name="T"/> on each object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="position"></param>
    /// <param name="range"></param>
    /// <param name="layerMask"></param>
    /// <param name="sortByDistance"></param>
    /// <param name="useRigidbody">When true, the component will be searched for on the attached rigidbody (and objects without one will be ignored).</param>
    /// <returns></returns>
    public static List<T> GetComponentsInRange<T>(Vector3 position, float range, LayerMask layerMask, bool sortByDistance = false, bool useRigidbody = true) where T : Component
	{
		var collidersInRange = Physics.OverlapSphere(position, range, layerMask);
		List<T> componentsInRange = new List<T>();

		foreach (Collider collider in collidersInRange)
		{
			if (useRigidbody)
			{
				if (collider.attachedRigidbody != null && collider.attachedRigidbody.TryGetComponent(out T component))
					componentsInRange.Add(component);
			}
			else
			{
				if (collider.TryGetComponent(out T component))
					componentsInRange.Add(component);
			}
		}

		if (sortByDistance)
			componentsInRange.Sort((a, b) => Vector3.SqrMagnitude(a.transform.position - position).CompareTo(Vector3.SqrMagnitude(b.transform.position - position)));


		return componentsInRange;
	}

	/// <summary>
	/// Uses <see cref="Physics.OverlapSphereNonAlloc(Vector3, float, Collider[], int)"/> to get objects in range, then looks for <typeparamref name="T"/> on each object.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="position"></param>
	/// <param name="range"></param>
	/// <param name="overlapResults"></param>
	/// <param name="layerMask"></param>
	/// <param name="componentsInRange"></param>
	/// <param name="sortByDistance"></param>
	/// <param name="useRigidbody"></param>
	/// <returns></returns>
	public static List<T> GetComponentsInRangeNonAlloc<T>(Vector3 position, float range, ref Collider[] overlapResults, LayerMask layerMask, ref List<T> componentsInRange, bool sortByDistance = false, bool useRigidbody = true) where T : Component
	{
		var collidersInRange = Physics.OverlapSphereNonAlloc(position, range, overlapResults, layerMask);
		for (int i = 0; i < collidersInRange; i++)
		{
			Collider collider = overlapResults[i];
			if (useRigidbody)
			{
				if (collider.attachedRigidbody != null && collider.attachedRigidbody.TryGetComponent(out T component))
					componentsInRange.Add(component);
			}
			else
			{
				if (collider.TryGetComponent(out T component))
					componentsInRange.Add(component);
			}
		}

		if (sortByDistance)
			componentsInRange.Sort((a, b) => Vector3.SqrMagnitude(a.transform.position - position).CompareTo(Vector3.SqrMagnitude(b.transform.position - position)));

		return componentsInRange;
	}

    /// <summary>
    /// Gets a world position for the mouse cursor. Returns null when raycasting hits an object on the blocking layer, UI, nothing.
    /// </summary>
    /// <param name="targetLayerMask">The layer mask for valid objects</param>
    /// <param name="blockingLayerMask">The layer mask for objects that block raycasts</param>
    /// <returns></returns>
    public static Vector3? GetWorldMousePosition(LayerMask targetLayerMask, LayerMask blockingLayerMask)
	{
		LayerMask combinedLayerMask = targetLayerMask.Add(blockingLayerMask);
		bool isOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

		if (!isOverUI)
		{
			// Cast a ray from the mouse position to the ground underneath to get a ground position
			Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, float.MaxValue, combinedLayerMask) && targetLayerMask.Contains(hit.transform.gameObject))
			{
				return hit.point;
			}
		}

		return null;
	}

	/// <summary>
	/// Offsets a position from other colliders in range
	/// </summary>
	/// <param name="position">position to offset</param>
	/// <param name="minDistance">offset amount</param>
	/// <param name="layerMask">layer mask used with Physics.OverlaySphere to get colliders in range</param>
	/// <returns></returns>
	public static Vector3 GetOffsetPosition(Vector3 position, float minDistance, LayerMask layerMask)
	{
		Vector3 offsetPosition = Vector3.zero;

		// Create a utility SphereCollider to use with concave mesh colliders
		if (offsetPositionSphereCollider == null)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.name = "Utilities.offsetPositionSphereCollider";
			gameObject.hideFlags = HideFlags.HideInHierarchy;
			offsetPositionSphereCollider = gameObject.AddComponent<SphereCollider>();
			offsetPositionSphereCollider.isTrigger = true;
		}

		offsetPositionSphereCollider.gameObject.SetActive(true);
		offsetPositionSphereCollider.transform.position = position;
		offsetPositionSphereCollider.radius = minDistance;


		Collider[] colliders = Physics.OverlapSphere(position, minDistance, layerMask);

		foreach (Collider collider in colliders)
		{
			Vector3 colliderPositionOffset = Vector3.zero;
			MeshCollider colliderAsMeshCollider = collider as MeshCollider;

			// For concave mesh colliders, use offsetPositionSphereCollider and Physics.CalculatePenetration to get the offset
			// Otherwise use Collider.ClosestPoint
			if (colliderAsMeshCollider != null && !colliderAsMeshCollider.convex)
			{
				if (Physics.ComputePenetration(offsetPositionSphereCollider, offsetPositionSphereCollider.transform.position, offsetPositionSphereCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out Vector3 direction, out float distance))
				{
					colliderPositionOffset = position + (direction * distance);
				}
			}
			else
			{
				Vector3 closestPoint = collider.ClosestPoint(position);
				float distance = Vector3.Distance(position, closestPoint);

				if (distance < minDistance)
				{
					Vector3 direction = (position - closestPoint).normalized;

					colliderPositionOffset = position + (direction * (minDistance - distance));
				}
			}
			
			offsetPosition += colliderPositionOffset;
		}

		// Average the offset positions
		if (colliders.Length > 0)
			offsetPosition /= colliders.Length;
		else
			offsetPosition = position;

		
		offsetPositionSphereCollider.gameObject.SetActive(false);


		return offsetPosition;
	}
	public static SphereCollider offsetPositionSphereCollider;
}