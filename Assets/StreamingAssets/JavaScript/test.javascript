(function createagameobject()
{
	var v = new GameObject();

	var watch = new Stopwatch();
	watch.Start();
	
	for (var i=0; 100000>i; i++)
	{
		//v.transform.position=Vector3.forward
		//v.SetActive(false)
		//v.isStatic = false
		var position= new Vector3(0.1, 0.1, 2.1);
		//v.tag="Player"
	}
	watch.Stop();
	printInt(watch.ElapsedMilliseconds)
	//v.name="yes i love you"
})()