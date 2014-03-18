WindowsHDP
==========

Automated console installer for Hortonworks Data Platform on windows. Can be used to install multi-node clusters, uninstall clusters or add nodes to an existing cluster.



Overview
========

This installer can be used to deploy HDP windows multi-node clusters, uninstall HDP from multi node machines and add new nodes and services to existing clusters. The installer is a console application written in .NET that utilizes Powershell 3.0 capabilities for invoking remote commands. This application has been tested with Server 2008 R2 (should work on 2008) and Server 2012.

Structure
=========

The prebuilt package for installation is located at https://github.com/acesir/WindowsHDP/tree/master/PrebuiltPackage abd contains all the files you need to deploy HDP 2.0 on Windows except the HDP msi package (is too large) which will need to downloaded manually and added. In the WinHDP folder you fill find the executable for the console application called WinHDP.exe and in the Files directory you will find all the required software (except HDP msi) files as well as a sample clusterproperties file which will need to be edited based on the cluster you are deploying too.


Step-ByStep Use
===============

* Download prebuilt package from here https://github.com/acesir/WindowsHDP/tree/master/PrebuiltPackage
* You will have to download HDP 2.0 for Windows (you can drop it in Files directory of the above or anywhere else)
* Disable Firewall on all Nodes
* For server 2008 make sure to install .NET Framework 4.0 (dotNetFx40_Full_Setup) and Powershell 3.0 (Windows6.1-KB2506143) on all Nodes (These files can be found in the Files directory)
* Open WinHDP.exe.conf file and populate\update required fields such as HDPDir, Hadoop password and File locations\ names
* Edit the clusterproprties file with target install cluster information
* Run installer with Administrator account that has Admin level permissions on all Nodes in the cluster
* Choose Install, Uninstall or Add Nodes
 

Add Node(s) to an existing cluster
==================================

* Choose 3. Add Node
* Add nodes and services in the following format $servicename=nodename1,nodename2|$servicename=nodename1
* $servicename needs to be the service name from the clusterproperties.txt file
* example: $SLAVE_HOSTS=node3,node4,node5|$FLUME_HOSTS=node3,node4|$HBASE_REGIONSERVERS=node5
* finally, hit enter and wait for the installation to finish
