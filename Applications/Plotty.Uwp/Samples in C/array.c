int main()
{
    int a;
    int b=0;
    int array[10];
    
    array[0] = 2;
    array[1] = 5;
    array[2] = 3;
    array[3] = 67;
    array[4] = 111;
    array[5] = 99;
    array[6] = 23;
    array[7] = 2;
    array[8] = 64;
    array[9] = 9;
    
    for (a=0; a<10; a=a+1) 
    {
        b = b + array[a];
    }
    
    return b;
}