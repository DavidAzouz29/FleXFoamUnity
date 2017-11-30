///<summary>
/// Name: BranchControllerDebug.cs
/// Author: David Azouz
/// Date Created: 17/10/17
/// Date Modified: 18/10/17
/// --------------------------------------------------
/// Brief: Xbox controller used to control branch.
/// viewed: 
/// 
/// --------------------------------------------------
/// Edits:
/// - Script Created - David Azouz 14/10/17
/// - Branch Anim based off Xbox - David Azouz 17/10/17
/// - Water properties adjusted - David Azouz 18/10/17
/// - Moved Input to Input Controller - David Azouz 18/10/17
/// 
/// TODO:
/// </summary>

using UnityEngine;

public class BranchControllerDebug : MonoBehaviour
{
    private float moveSpeed = 0.3f;
    private float rotSpeed = 45f;

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(
            Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime,
            0,
            Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime);
        transform.Rotate(
            Input.GetAxis("Rot Y") * rotSpeed * Time.deltaTime,
            Input.GetAxis("Rot X") * rotSpeed * Time.deltaTime, 
            0, Space.Self);

    }
}
