/*
* this is a simple callback js
* functions (except delegate function) are called with 'this' set to a GameObject
*/

// these 2 variables belongs to 'this' object
// 

//globalObj = new GameObject('JZYX')

var mTrans = undefined
var rotateVar = undefined
var fAccum = 0

function Awake()
{
    // var v = new GameObject('Micheal Jackson');
    //mTrans = transform;

    //Debug.Log('Awake called');
}
function Start()
{
}

function Update()
{
    // here
    // 'this' means a GameObject

    // rotate gameobject
    if (mTrans === undefined)
    {
        mTrans = transform;
        rotateVar = new Vector3(0.5, 0.5, 0);
    }
    mTrans.Rotate(rotateVar);

    if (Input.GetMouseButtonDown(0) === true)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hit = new RaycastHit();
        Physics.Raycast(ray, hit);
        var t = hit.transform;
        if (t === undefined)
        {
            Debug.Log('Hit nothing')
        }
        else
        {
            Debug.Log('Hit ' + t.name);
        }
    }

    fAccum += Time.deltaTime
    if (fAccum > 5)
    {
       UnityObject.Destroy(this);
    }
}

function Destroy()
{
    Debug.Log('js Destroy Called!')
}