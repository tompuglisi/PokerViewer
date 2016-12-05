using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Owin;
using Owin;

namespace PokerViewer
{
   
	public static class DefaultValues
{
  
  public static SelectList ItemsPerPageList 
    { 
      get 
      { 
		 
		  return (new SelectList( new List<int>
            {
                5,
                10,
               25,
                50,
                100,
              
            },selectedValue: 10)); 
      } 
    }
  
}
}
