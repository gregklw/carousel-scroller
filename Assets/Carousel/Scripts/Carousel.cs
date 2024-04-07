using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Carousel : MonoBehaviour
{
    private GraphicRaycaster _graphicRaycaster;
    private EventSystem _eventSystem;
    private PointerEventData _pointerEventData;
    private bool _isTimerOn;
    private Vector3 _previousMousePosition;
    private float _xDragDelta;
    private float _xDragDistanceValue;

    [SerializeField] private RectTransform _memberParent, _middleFocus;

    CarouselMember _firstMember, _lastMember, _centerMemberOnScreen;

    [Header("Carousel Settings")]
    [SerializeField] private float _carouselSpacing;
    [SerializeField] private float _carouselMemberSize;

    // Start is called before the first frame update
    void Start()
    {
        _graphicRaycaster = GetComponentInParent<GraphicRaycaster>();
        CarouselMember[] members = _memberParent.GetComponentsInChildren<CarouselMember>();
        _firstMember = members[0];
        _lastMember = members[members.Length - 1];

        float carouselIncrementValue = 0;

        foreach (CarouselMember member in members)
        {
            member.transform.position = _middleFocus.position + Vector3.right * _carouselSpacing * carouselIncrementValue;
            member.SetSize(_carouselMemberSize);
            carouselIncrementValue++;
        }
        _pointerEventData = new PointerEventData(_eventSystem);
    }

    private void FixedUpdate()
    {
        if (_isTimerOn)
        {
            _xDragDelta = (Input.mousePosition.x - _previousMousePosition.x);
            Debug.Log(_xDragDelta);
            _xDragDistanceValue += Mathf.Abs(_xDragDelta);
        }
        Scroll();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _pointerEventData.position = _middleFocus.transform.position;
            _centerMemberOnScreen = GetTopMemberOnRaycast(_pointerEventData);

            _pointerEventData.position = Input.mousePosition;
            _previousMousePosition = Input.mousePosition;

            if (GetTopMemberOnRaycast(_pointerEventData) != null)
            {
                TurnOnTimer();
            }
        }
        else if (Input.GetMouseButton(0))
        {
            _previousMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            DetermineActionOnRelease();
            ResetTimer();
        }
    }

    private void LateUpdate()
    {
        
    }

    private void TurnOnTimer()
    {
        _isTimerOn = true;
    }

    private void ResetTimer()
    {
        _isTimerOn = false;
        _xDragDistanceValue = 0;
        _xDragDelta = 0;
    }

    private void Scroll()
    {
        if (_lastMember.transform.position.x + _xDragDelta <= _middleFocus.position.x && _xDragDelta < 0)
        {
            _xDragDelta = _middleFocus.position.x - _lastMember.transform.position.x;
        }
        else if (_firstMember.transform.position.x + _xDragDelta >= _middleFocus.position.x && _xDragDelta > 0)
        {
            _xDragDelta = _middleFocus.position.x - _firstMember.transform.position.x;
        }
        _memberParent.position += new Vector3(_xDragDelta, 0, 0);
    }

    private void DetermineActionOnRelease()
    {
        if (_xDragDistanceValue > 0)
        {
            CarouselMember memberToBeCentered = DetermineClosestMemberToPoint(_middleFocus);
            SnapMemberToTarget(_middleFocus, memberToBeCentered);
        }
        else
        {
            _pointerEventData.position = Input.mousePosition;
            CarouselMember targetMember = GetTopMemberOnRaycast(_pointerEventData);

            if (_centerMemberOnScreen == targetMember)
            {
                _centerMemberOnScreen.RunCarouselMember();
            }
            else
            { 
                SnapMemberToTarget(_middleFocus.transform, targetMember);
            }
        }
    }

    private void SnapMemberToTarget(Transform targetPoint, CarouselMember targetMember)
    {
        if (targetMember != null)
        {
            Vector3 translationAmount = targetPoint.position - targetMember.transform.position;
            _memberParent.position += translationAmount;
        }
    }

    private CarouselMember DetermineClosestMemberToPoint(Transform targetPoint)
    {
        _pointerEventData.position = targetPoint.position + Vector3.left * (_carouselSpacing - _carouselMemberSize) / 2;
        CarouselMember leftMember = GetTopMemberOnRaycast(_pointerEventData);

        _pointerEventData.position = targetPoint.position + Vector3.right * (_carouselSpacing - _carouselMemberSize) / 2;
        CarouselMember rightMember = GetTopMemberOnRaycast(_pointerEventData);

        CarouselMember closestMember;

        if (leftMember == null)
        {
            closestMember = rightMember;
        }
        else if (rightMember == null)
        {
            closestMember = leftMember;
        }
        else
        {
            float leftDistance = Vector3.SqrMagnitude(leftMember.transform.position - targetPoint.position);
            float rightDistance = Vector3.SqrMagnitude(rightMember.transform.position - targetPoint.position);


            if (leftDistance <= rightDistance)
            {
                closestMember = leftMember;
            }
            else
            {
                closestMember = rightMember;
            }
        }

        return closestMember;
    }

    private CarouselMember GetTopMemberOnRaycast(PointerEventData pointerEventData)
    {
        CarouselMember memberToReturn = null;

        List<RaycastResult> results = new List<RaycastResult>();

        _graphicRaycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            memberToReturn = result.gameObject.GetComponent<CarouselMember>();
            if (memberToReturn) return memberToReturn;
        }

        return null;
    }
}
