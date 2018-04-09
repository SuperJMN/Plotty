void main() 
{
	f=fibonacci();
	return;
}

int fibonacci()
{
	n = 12;
	first = 0;
	second = 1;
	
	for (c = 0; c<n ;c=c+1)
	{
		if ( c <= 1)
		{
			next = c;
		}
		else
		{
			next = first + second;
			first = second;
			second = next;
		}      
	}

	return next;
}