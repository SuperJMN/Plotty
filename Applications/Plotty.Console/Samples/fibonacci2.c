Main()
{
	n = 12;
	first = 0;
	second = 1;
	
	for (c = 0; c<n ;c=c+1)
	{
		if ( c < 2 )
		{
			next = c;
		}

		if ( c > 1)
		{
			next = first + second;
			first = second;
			second = next;
		}      
	}
}