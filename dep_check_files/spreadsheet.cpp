#include <iostream>
#include <string.h>
#include <map>
#include "spreadsheet.h"
#include <fstream>
#include <sys/types.h>
#include <sys/stat.h>
#include <cstring>
#include <cctype>
#include <stdio.h>



spreadsheet::spreadsheet()
{
  
}

spreadsheet::spreadsheet(std::string filename)
{
  load_spreadsheet(filename);
}

//when is a copy constructor needed 
spreadsheet::spreadsheet(const spreadsheet &other)
{

}

spreadsheet::~spreadsheet()
{
}

/*
 
  Checks for dependencies between the cell and the formula placed in it. 
 
*/
set<char*> spreadsheet::Dependency_Check(set<char*> current_deps, char* name, char* formula, int* truth)
{

std::string name_holder(name); //convert name to a string 

 char* temp = NULL;

 char* test = strdup(formula); // this line of code only works with posix it use to convert const char* to char* 

 

//std::cout<< "Got to the token parser"<< "formula: " << formula << std::endl; 


temp = strtok(test, " =+-/*");//parses tokens from equation 
//std::cout << "Got past the first parse:" << temp << std::endl;

//parses the entire equation  
 while(temp != NULL){
//std::cout << "in while loop" << std::endl;
//std::cout << isalpha(temp[0]) << std::endl;  

//checks that the first character of var is an letter 
//and that the string is either two or three characters long 
if(isalpha(temp[0]) && (strlen(temp) == 3 || strlen(temp) == 2)){

std::string holder(temp);

if(holder == name_holder){ // if var name is same as the cell name circular dependency 

//std::cout << "arrived at dependency" << std::endl; 

*truth = 1; //dep found 

return current_deps; 

}

//std::cout<< holder<< std::endl; 

if(depgraph.HasDependents(holder)){ //get the cells the var is dependent on 


set<std::string> temp_deps = depgraph.GetDependents(holder);

//go through each element of dependents and if it has not already been looked at 
// make a recursive call to Dependency_Check 
for(set<std::string>::iterator it = temp_deps.begin(); it != temp_deps.end(); it++){

if(!current_deps.count( const_cast<char*> (it->c_str()))); 
{

current_deps.insert(const_cast<char*> (it->c_str())); 

std::string temp_name;

temp_name = "=" + *it; 

char* c_name  = const_cast<char*> (temp_name.c_str());//converts temp name to c string  

//std::cout<< "got to recursive call" << std::endl;
current_deps = Dependency_Check(current_deps, name,c_name, truth); 

if(*truth == 1){ // if a circular dependency has been found return true

return current_deps; 

}

}


}


}




}


temp = strtok(NULL, " =+-/*");
 

 }

return current_deps; 


}

void spreadsheet::load_spreadsheet(std::string filename)
{
  
   string File = "../Saves";//one dot for local two for parent 
 
   string cell_info; 

   char* cell_name; 

   char* cell_contents; 
  
   //temps 

  //map<string,string> cells;
  
   //string name = "test_cell_data.txt"; // needs to be a read in as a parameter 

   //end temps 
/* 
//  heavily modified code from Ingo Leonhardt but seems really basic once you understand it http://stackoverflow.com/questions/18100097/portable-way-to-check-if-directory-exists-windows-linux-c  
   struct stat info;

   if( stat( File.c_str(), &info ) != 0 )//changed pathname to file 
       printf( "cannot access %s\n", File.c_str());
   else if( info.st_mode & S_IFDIR ) 
       printf( "%s is a directory\n", File.c_str());
   else{
       printf( "%s is no directory creating one \n", File.c_str()); // modified 
       mkdir(File.c_str(), S_IRWXU | S_IRWXG | S_IROTH | S_IXOTH); // my line of code 
       }
      
//end of cited code 
*/
   string full_name = filename;    
   cout << full_name << endl;

    ifstream save_file (full_name.c_str()); 

    if(save_file.is_open()){
        
        while( getline (save_file, cell_info) ){

             cout << "got here" << endl; 
             char temp[cell_info.length()];
  
             strcpy (temp, cell_info.c_str()); 
             cell_name = strtok(temp, "|");
	     cout << cell_name << endl;

             cell_info= strtok(NULL, "|");
              cout << cell_info << endl;
             cout << "got further" << endl; 
	     string name(cell_name);
	     string info(cell_info);
             cells.insert ( make_pair(name, info));
//             cells.insert ( make_pair(cell_name, cell_contents));
cout <<"me too"<<endl;

               
        }
        
 
        }else{
       
	     //should send errror message 
         cout << "file_not_found" << endl; 

        }
         
       int total_cells = cells.size(); 

       cout << total_cells << endl; 

}

void spreadsheet::save_spreadsheet(std::string filename)
{
 string File = "../Saves";// one dot local two for parent 
 
   string cell_info; 

   string cell_name; 

   string cell_contents; 
  
   //temps 

   //map<string,string> cells;
  
   //string name = "save_cell_test.txt"; // would be paramater 

   //end temps 
 
//  heavily modified code from Ingo Leonhardt but seems really basic once you understand it http://stackoverflow.com/questions/18100097/portable-way-to-check-if-directory-exists-windows-linux-c  
   struct stat info;

   if( stat( File.c_str(), &info ) != 0 )//changed pathname to file 
       printf( "cannot access %s\n", File.c_str());
   else if( info.st_mode & S_IFDIR ) 
       printf( "%s is a directory\n", File.c_str());
   else{
       printf( "%s is no directory creating one \n", File.c_str()); // modified 
       mkdir(File.c_str(), S_IRWXU | S_IRWXG | S_IROTH | S_IXOTH); // my line of code 
       }
      
//end of cited code 
  
   string full_name = File + filename;    
   /*
   cells.insert ( pair<string,string>("a1","hello"));
   cells.insert ( pair<string,string>("b1","32"));
   cells.insert ( pair<string,string>("c3","=10+1"));
   */
    ofstream save_file (full_name.c_str()); 

    if(save_file.is_open()){
        
        for (map<string,string>::iterator it = cells.begin(); it != cells.end(); ++it){
            
            cell_name = it->first;

            cell_contents = it->second; 

            string temp1(cell_name); 
       
            string temp2(cell_contents); 

            string cell_info = temp1 + "|" + temp2; 

            save_file << cell_info << endl;   


        }
 
        }else{
      
	     //should really return an error message. 
         cout << "File could not be opened" << endl;  

        }
  
}

