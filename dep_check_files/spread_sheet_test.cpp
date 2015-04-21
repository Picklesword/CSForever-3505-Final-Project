#include "spreadsheet.h"
#include "Dependency_Graph.h"
#include <iostream>
#include <string> 
#include <set> 

int main(int argv, char* argch[]){
spreadsheet test_sheet; 

test_sheet.depgraph.AddDependency("a1","c1"); 

test_sheet.depgraph.AddDependency("c1","e1"); 

test_sheet.depgraph.AddDependency("e1","b2"); 


test_sheet.depgraph.AddDependency("c1","d1"); 

test_sheet.depgraph.AddDependency("d1","b1");


std::set<char*> temp_deps; 

int temp; 

test_sheet.Dependency_Check(temp_deps, const_cast<char*>("b2"), const_cast<char*>("=24 + a1 + 24"), &temp); 

if(temp ==1){
std::cout << "it worked" << endl;
}



return 0; 

}

