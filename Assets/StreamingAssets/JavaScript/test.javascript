(function createagameobject()
{
	var v = new GameObject();

	var watch = new Stopwatch();
	watch.Start();
	for (var i=0; 100000>i; i++)
	{
		v.SetActive(false)
	}
	watch.Stop();
	printInt(watch.ElapsedMilliseconds)
})()