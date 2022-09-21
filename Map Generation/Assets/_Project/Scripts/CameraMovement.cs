using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration
{
	public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float m_MoveSpeed;
		private Vector3 m_Input;

		private void Start()
		{
			m_Input = Vector3.zero;
		}

		private void Update()
		{
			GetInput();
			MoveCamera();
		}

		private void GetInput()
		{
			m_Input.x = Input.GetAxis("Horizontal");
			m_Input.z = Input.GetAxis("Vertical");
		}

		
		private void MoveCamera()
		{
			transform.position += m_MoveSpeed * Time.deltaTime * m_Input.z * transform.forward;
			transform.position += m_MoveSpeed * Time.deltaTime * m_Input.x * transform.right;
		}
	}
}
