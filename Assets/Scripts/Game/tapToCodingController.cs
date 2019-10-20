using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class tapToCodingController : MonoBehaviour
{

    const string PYTHONCODE = @"
def partition(arr,low,high): 
    i = ( low-1 )
    pivot = arr[high]
    for j in range(low , high): 
        if   arr[j] <= pivot: 
            i = i+1 
            arr[i],arr[j] = arr[j],arr[i] 
  
    arr[i+1],arr[high] = arr[high],arr[i+1] 
    return ( i+1 ) 

def quickSort(arr,low,high): 
    if low < high: 
        pi = partition(arr,low,high) 
        quickSort(arr, low, pi-1) 
        quickSort(arr, pi+1, high) 
    ";

    private int count = 0;
    private int codeLength = 0;

    public Text output;
    // Start is called before the first frame update
    void Start()
    {
        codeLength = PYTHONCODE.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (count < codeLength){
            output.text = PYTHONCODE.Substring(0,count);
        }
    }

    public void tap(){
         if (count < codeLength){
            count += 4;
            while(PYTHONCODE[count] == ' '){
                count+=1;
            }
         }
         else{
             count = codeLength;
         }
        
    
    }
}
