function Awake()
{
	
}

function Update()
{
	if (this.mTrans === undefined)
	{
		this.mTrans = this.transform;
		this.rotateVar = new Vector3(0.5, 0, 0);
	}
	this.mTrans.Rotate(this.rotateVar);
}