/*
Dependency_Graph.cpp
by Jonathan Warner based off DependencyGraph.cs from cs3500

Data structure is used to map dependencies between variables.

*/
#include "Dependency_Graph.h"
#include <iostream>
#include <string>

/*

 Constructor for the Dependency_Graph

 */
Dependency_Graph::Dependency_Graph(){

	pair_count = 0;
	dependees = "_dependees"; // used to indicate the dependees set 
	dependents = "_dependents";//used to indicate the dependents set   

}
/*

 Deconstructor for Dependency_Graph

 */
Dependency_Graph::~Dependency_Graph(){

}

/*

 Returns the amount of dependency pairs there are

 */

int Dependency_Graph::Size(){

	return pair_count;

}

/*

 Given a variable it returns the amount of variables that
 Depend on it.


 */


int Dependency_Graph::size_dependees(std::string dependent){

	std::map<std::string, std::set<std::string> >::iterator temp_it;

	temp_name = dependent + dependees;

	temp_it = omni_set.find(temp_name); //finds the set of dependees for temp name 

	//checks to sees if the set of dependees exists 
	if (temp_it == omni_set.end()){
		return 0;
	}


	return temp_it->second.size();//returns size of set of dependees   
}

/*

Checks if the variable has dependents

*/

int Dependency_Graph::HasDependents(std::string s){

	std::map<std::string, std::set<std::string> >::iterator temp_it;

	temp_name = s + dependents;

	temp_it = omni_set.find(temp_name);

	//if set of dependents does not exists return false 
	if (temp_it == omni_set.end()){
		return 0;
	}

	return 1; //else return true  

}

/*

 Tests if variable has dependees

 */

int Dependency_Graph::HasDependees(std::string s){

	std::map<std::string, std::set<std::string> >::iterator temp_it;


	temp_name = s + dependees;

	temp_it = omni_set.find(temp_name);

	//if no set of dependees return false 
	if (temp_it == omni_set.end()){
		return 0;
	}

	//else return true 
	return 1;

}

/*
 Returns the set of dependents for a variable
 */

std::set<std::string> Dependency_Graph::GetDependents(std::string s){

	std::map<std::string, std::set<std::string> >::iterator temp_it;

	temp_name = s + dependents;

	temp_it = omni_set.find(temp_name);

	if (temp_it == omni_set.end()){
		// dependents do not exits  

	}

	// std::cout << "Got here" << std::endl;  

	return omni_set[temp_name];//return set of dependents   
}

/*

 Returns set of dependees for variable

 */

std::set<std::string> Dependency_Graph::GetDependees(std::string s){

	std::map<std::string, std::set<std::string> > ::iterator temp_it;

	temp_name = s + dependees;

	temp_it = omni_set.find(temp_name);

	if (temp_it == omni_set.end()){
		//set does not exist  
	}


	return omni_set[temp_name]; //return set of dependees  
}

/*

 Adds a new dependency to the data structure

 */

void Dependency_Graph::AddDependency(std::string s, std::string t){

	std::map<std::string, std::set<std::string> >::iterator temp_it;

	std::set<std::string>::iterator temp_set;

	std::set<std::string> temp_set2;

	temp_name = s + dependents;

	//adds t to the dependents list for s 

	temp_it = omni_set.find(temp_name); // checks if set already exists 

	//if it exists add the new dependent to the set 
	if (temp_it != omni_set.end()){

		temp_set = temp_it->second.find(t);

		//checks to make sure the dependency does not already exists 
		if (temp_set == temp_it->second.end()){


			temp_it->second.insert(t);

			pair_count++;


		}
	}
	else{

		//creates a new set if a dependents set did not exist for s 
		temp_set2.insert(t);

		omni_set.insert(std::pair<std::string, std::set<std::string> >(temp_name, temp_set2));

		pair_count++;

	}

	// adds s to the dependees list for t 
	temp_name = t + dependees;

	temp_it = omni_set.find(temp_name);

	//std::cout << "temp name is " + temp_name << std::endl;

	//checks to see if the dependees set already exists 
	if (temp_it != omni_set.end()){

		temp_set = temp_it->second.find(s);

		if (temp_set == temp_it->second.end()){


			temp_it->second.insert(s);//adds s to dependees list for t  

			//std::cout<< "Does not contain" + s << std::endl; 





		}
	}
	else{ //creates a new set of dependees did not exist for t 

		temp_set2.clear();
		temp_set2.insert(s);
		//std::cout<< "Does not contain" + s << std::endl;
		omni_set.insert(std::pair<std::string, std::set<std::string> >(temp_name, temp_set2));



	}

}

/*

 Removes dependency pairs form dependency graph

 */
void Dependency_Graph::RemoveDependency(std::string s, std::string t){

	std::map<std::string, std::set<std::string> >::iterator temp_it;

	std::set<std::string>::iterator temp_set;

	//first remove t from s dependents set 

	temp_name = s + dependents;

	temp_it = omni_set.find(temp_name);

	//makes sure the set exists 
	if (temp_it != omni_set.end()){

		temp_set = temp_it->second.find(t);

		if (temp_set != temp_it->second.end()){

			temp_it->second.erase(t);//removes t from set 



			pair_count--;


		}
	}

	//remove s from t dependees set 

	temp_name = t + dependees;

	temp_it = omni_set.find(temp_name);

	//make sure set exists 
	if (temp_it != omni_set.end()){

		temp_set = temp_it->second.find(s);

		if (temp_set != temp_it->second.end()){

			temp_it->second.erase(s);//gets rid of s from set 





		}
	}

}


/*

 Replaces the current dependents set for s with a new one

 */
void Dependency_Graph::ReplaceDependents(std::string s, std::set<std::string> newDependents){


	std::map<std::string, std::set<std::string> >::iterator temp_it;

	std::set<std::string>::iterator temp_set;

	std::map<std::string, std::set<std::string> >::iterator  temp_set2;

	std::set<std::string>::iterator temp_set3;

	//access the set of dependents 
	temp_name = s + dependents;


	temp_it = omni_set.find(temp_name);

	//make sure it exists 
	if (temp_it != omni_set.end()){

		//iterate through the old set and remove s from each elements dependees set
		for (temp_set = temp_it->second.begin(); temp_set != temp_it->second.end(); temp_set++){

			pair_count--; //pair is gone 

			temp_name = *temp_set + dependees;

			//find the dependees set for the element of dependents 
			temp_set2 = omni_set.find(temp_name);

			if (temp_set2 != omni_set.end()){

				//find s in the set of dependees 
				temp_set3 = temp_set2->second.find(s);

				if (temp_set3 != temp_set2->second.end()){

					temp_set2->second.erase(s);//delete s from the set 


				}


			}
		}


	}


	temp_name = s + dependents;

	//find dependents set for s 
	temp_it = omni_set.find(temp_name);

	// make sure set exists 
	if (temp_it != omni_set.end()){

		temp_it->second.clear(); //delete all the contents of the set 

		//for each element of the replacement set add it and s to dependency graph 
		for (temp_set = newDependents.begin(); temp_set != newDependents.end(); temp_set++){

			AddDependency(s, *temp_set);

		}

	}
	else{ // if set did not exist no need to clear it 

		//create a dependency between s and each element of the new dependents set 
		for (temp_set = newDependents.begin(); temp_set != newDependents.end(); temp_set++){

			AddDependency(s, *temp_set);

		}


	}

}

/*

 Replaces the dependees set for s with a new set of dependees

 */
void Dependency_Graph::ReplaceDependees(std::string s, std::set<std::string> newDependees){


	std::map<std::string, std::set<std::string> >::iterator temp_it;

	std::set<std::string>::iterator temp_set;

	std::map<std::string, std::set<std::string> >::iterator  temp_set2;

	std::set<std::string>::iterator temp_set3;

	temp_name = s + dependees;

	//find the set of dependees 
	temp_it = omni_set.find(temp_name);

	if (temp_it != omni_set.end()){
		//remove s from the dependents set for each element in the current dependees for s 
		for (temp_set = temp_it->second.begin(); temp_set != temp_it->second.end(); temp_set++){

			pair_count--; // pair gone 

			temp_name = *temp_set + dependents;

			temp_set2 = omni_set.find(temp_name);

			if (temp_set2 != omni_set.end()){

				temp_set3 = temp_set2->second.find(s);

				if (temp_set3 != temp_set2->second.end()){

					temp_set2->second.erase(s);



				}


			}
		}


	}

	//get dependees set for s 
	temp_name = s + dependees;

	temp_it = omni_set.find(temp_name);

	//delete dependees set and then add each dependency for each element in new set of dependees and s 
	if (temp_it != omni_set.end()){

		temp_it->second.clear();

		for (temp_set = newDependees.begin(); temp_set != newDependees.end(); temp_set++){

			AddDependency(*temp_set, s);

		}

	}
	else{ // if set did not exist just add dependency between each element of new dependees and s 

		for (temp_set = newDependees.begin(); temp_set != newDependees.end(); temp_set++){

			AddDependency(*temp_set, s);

		}

	}

}






