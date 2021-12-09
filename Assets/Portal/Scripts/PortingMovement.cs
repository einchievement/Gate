using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortingMovement : MonoBehaviour
{
    public GameObject cloneGameObject;

    public Vector3 PortingDirection { get; set; }
    public Vector3 PortingFromDirection { get; set; }
    public Transform Clone { get => clone; set => clone = value; }
    public Transform OriginalPortal { get => originalPortal; set => originalPortal = value; }
    public Transform ClonePortal { get => clonePortal; set => clonePortal = value; }

    private Rigidbody rb;
    private Vector3 lastvelocity;
    private Transform clone, originalPortal, clonePortal;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        lastvelocity = rb.velocity;
    }

    public void DestroyClone()
    {
        if (clone != null)
        {
            Destroy(clone.gameObject);
        }
    }

    public void TransferControlToClone()
    {
        IPortable clonePortable = clone.GetComponent<IPortable>();
        clonePortable.Declonify(gameObject);
        clonePortable.CurrentPortingState = IPortable.PortingState.InProgress;
        clone.parent = null;
        clone.GetComponent<Rigidbody>().velocity = DeterminePortingVelocity();
        MakeToClone();
        //Assign Clone
        PortingMovement clonePm = clonePortable.PortingMvmnt;
        clonePm.Clone = transform;
        //Assign Portals
        clonePm.ClonePortal = originalPortal;
        clonePm.OriginalPortal = clonePortal;
    }

    public void MakeToClone()
    {
        gameObject.layer = 13; //ClonePlayer
        transform.parent = originalPortal;
        GetComponent<IPortable>().IsClone = true;
    }

    public void InstantiateClone(Transform originalPortal, Transform clonePortal)
    {
        if (clone != null)
        {
            return;
        }
        this.originalPortal = originalPortal;
        this.clonePortal = clonePortal;
        clone = Instantiate(cloneGameObject, clonePortal.position, Quaternion.identity).transform;
        clone.GetComponent<IPortable>().IsClone = true;
        clone.parent = clonePortal;
        clone.gameObject.layer = 13; //ClonePlayer
        UpdateClone();
    }

    public void UpdateClone()
    {
        UpdateClonePosition();
        UpdateCloneRotation();
    }

    private void UpdateClonePosition()
    {
        // determine relative position of original object to portal
        Vector3 distance = transform.position - originalPortal.position;
        Vector3 relativePosition = Vector3.zero;
        relativePosition.x = Vector3.Dot(distance, originalPortal.right) * -1;
        relativePosition.y = Vector3.Dot(distance, originalPortal.up) * -1;
        relativePosition.z = Vector3.Dot(distance, originalPortal.forward);
        // set position of clone relative to other portal
        clone.localPosition = relativePosition;
        //Debug.Log(relativePosition);
    }

    private void UpdateCloneRotation()
    {
        float playerAngleDiff = Vector3.SignedAngle(originalPortal.up * -1, transform.forward, Vector3.up);
        clone.forward = clonePortal.up;
        //clone.Rotate(new Vector3(0, playerAngleDiff * -1, 0), Space.Self);
    }

    private Vector3 DeterminePortingVelocity()
    {
        // find origin dir
        float x = Mathf.Abs(PortingFromDirection.x);
        float y = Mathf.Abs(PortingFromDirection.y);
        float z = Mathf.Abs(PortingFromDirection.z);
        float updater;
        if (x > y && x > z)
        {
            updater = Mathf.Abs(lastvelocity.x);
        }
        else if (y > z)
        {
            updater = Mathf.Abs(lastvelocity.y);
        }
        else
        {
            updater = Mathf.Abs(lastvelocity.z);
        }

        // find exit dir;
        return PortingDirection * updater;
    }
}