﻿using System.Collections.Generic;
using UnityEngine;

namespace ChooChoo
{
  public class TrainPositionDestination : ITrainDestination
  {
    private readonly TrainNavigationService _trainNavigationService;

    public TrainDestination Destination { get; }

    public TrainPositionDestination(TrainNavigationService trainNavigationService, TrainDestination destination)
    {
      _trainNavigationService = trainNavigationService;
      Destination = destination;
    }

    public bool GeneratePath(Transform transform,List<TrackRoute> pathCorners, bool isStuck)
    {
      var test = _trainNavigationService.FindRailTrackPath(transform, Destination, pathCorners, isStuck);
      // Plugin.Log.LogInfo("Found Path: " + test);
      return test;
    }
  }
}
