using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathingNode
{
    private PathingNode nextNode;
    private Transform destination;
    private int direction;
    private bool finalNode;
    public PathingNode(Transform destination, int direction, PathingNode nextNode){

        this.nextNode = nextNode;
        this.direction = direction;
        this.destination = destination;
        this.finalNode = false;
    }
    public PathingNode(Transform destination, Vector2 startPos, PathingNode nextNode){
        this.nextNode = nextNode;
        this.destination = destination;
        if (destination.position.x > startPos.x){
            this.direction = 1;
        }
        else{
            this.direction = -1;
        }
        this.finalNode = false;
    }
    public PathingNode(Transform destination, int direction){
        nextNode = null;
        this.direction = direction;
        this.destination = destination;
        this.finalNode = true;
    }
    public PathingNode(Transform destination, Vector2 startPos){
        nextNode = null;
        this.destination = destination;
        if (destination.position.x > startPos.x){
            this.direction = 1;
        }
        else{
            this.direction = -1;
        }
        this.finalNode = true;
    }
    public Transform getDestination(){
        return destination;
    }

    public int getDirection(){
        return direction;
    }

    public PathingNode getNextNode(){
        return nextNode;
    }

    public bool isFinal(){
        return finalNode;
    }

    public bool pastDest(Vector2 location){
        return ((location.x > destination.position.x && direction == 1) ||(location.x < destination.position.x && direction == -1));
    }

    public static PathingNode generatePathTo(Transform destination, Transform start, Floor[] floors){
        
        Vector2 destinationPos = destination.position;
        if (destinationPos.y == start.position.y){
            return new PathingNode(destination, start.position);
        }
        else{
            int destFloorIndex = 0;
            int floorOnIndex = 0;
            
            while (destFloorIndex+1 < floors.Length && floors[destFloorIndex+1].getHeight() < destinationPos.y){
                destFloorIndex++;
            }
            while (floorOnIndex+1 < floors.Length && floors[floorOnIndex+1].getHeight() < start.position.y){
                floorOnIndex++;
            }
            
            PathingNode pathToDest;
            Transform lastDest = start;
            Transform currentDest;
            if (destFloorIndex> floorOnIndex){
                currentDest = floors[floorOnIndex].getStairsUpStart();
                pathToDest = new PathingNode(currentDest, lastDest.position);
                lastDest = currentDest;
                currentDest = floors[floorOnIndex+1].getStairsDownStart();
                pathToDest = new PathingNode(currentDest, lastDest.position, pathToDest);
                lastDest = currentDest;
                floorOnIndex++;
                while (destFloorIndex != floorOnIndex){
                    currentDest = floors[floorOnIndex].getStairsUpStart();
                    pathToDest = new PathingNode(currentDest, lastDest.position);
                    lastDest = currentDest;
                    currentDest = floors[floorOnIndex+1].getStairsDownStart();
                    pathToDest = new PathingNode(currentDest, lastDest.position, pathToDest);
                    lastDest = currentDest;
                    floorOnIndex++;
                }
                return new PathingNode(destination, lastDest.position, pathToDest);
            }
            else if(destFloorIndex < floorOnIndex){
                currentDest = currentDest = floors[floorOnIndex].getStairsDownStart();
                pathToDest = new PathingNode(currentDest, lastDest.position);
                lastDest = currentDest;
                currentDest = floors[floorOnIndex-1].getStairsUpStart();
                pathToDest = new PathingNode(currentDest, lastDest.position, pathToDest);
                lastDest = currentDest;
                floorOnIndex--;
                while (destFloorIndex != floorOnIndex){
                    currentDest = currentDest = floors[floorOnIndex].getStairsDownStart();
                    pathToDest = new PathingNode(currentDest, lastDest.position);
                    lastDest = currentDest;

                    currentDest = floors[floorOnIndex-1].getStairsUpStart();
                    pathToDest = new PathingNode(currentDest, lastDest.position, pathToDest);
                    lastDest = currentDest;
                    floorOnIndex--;
                }
                return new PathingNode(destination, lastDest.position, pathToDest);
            }
            else{
                return new PathingNode(destination, start.position);
            }             
        }
    }
}
