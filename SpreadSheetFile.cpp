/*
 * This class servers as an spreadsheet object on the server side
 * This structure assumes that all validation of client input will be handled
 * directly in the main class
 *
 * Scott Young
 * Dharani Adhikari
 * Mathnew Greave
 * Jonathan Warner
 */

#include <string.h>
#include <stdlib.h>
#include <iostream>
#include "SpreadSheetFile.h"

using namespace std;

namespace FinalProject
{
  
    class SpreadSheet
    {
         //This is the constructor for spreadsheet class
         SpreadSheet::SpreadSheet(map <string, string> data, string name)
         {
         }
         // This function saves the committed cell value to the spreadsheet file
         void save(string cell, string content)
	 {
         }
         // This function deletes an entry to the spreadsheet file
         void delete(string cell, string content)
	 {
         }

         // This function is used to undo the most recent change on the spreadsheet
         void undo(void);
         {
         }
    };

}//end namespace
#endif
