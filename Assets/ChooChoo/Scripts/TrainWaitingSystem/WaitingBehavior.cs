﻿using Bindito.Core;
using System;
using Timberborn.BehaviorSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using UnityEngine;

namespace ChooChoo
{
  public class WaitingBehavior : RootBehavior, IDeletableEntity
  {
    private BlockService _blockService;
    private ClosestTrainWaitingLocationPicker _closestTrainWaitingLocationPicker;
    private MoveToStationExecutor _moveToStationExecutor;
    private WaitExecutor _waitExecutor;
    private TrainWaitingLocation _currentWaitingLocation;

    [Inject]
    public void InjectDependencies(BlockService blockService, ClosestTrainWaitingLocationPicker closestTrainWaitingLocationPicker)
    {
      _blockService = blockService;
      _closestTrainWaitingLocationPicker = closestTrainWaitingLocationPicker;
    }
    
    public void Awake()
    {
      _moveToStationExecutor = GetComponent<MoveToStationExecutor>();
      _waitExecutor = GetComponent<WaitExecutor>();
    }

    public override Decision Decide(GameObject agent)
    {
      var trainWaitingLocation = _blockService.GetFloorObjectComponentAt<TrainWaitingLocation>(transform.position.ToBlockServicePosition());
      if (_currentWaitingLocation != null && trainWaitingLocation == _currentWaitingLocation)
      {
        _waitExecutor.LaunchForIdleTime();
        return Decision.ReleaseWhenFinished(_waitExecutor);
      }
      if (trainWaitingLocation != null && !trainWaitingLocation.Occupied)
        return OccupyWaitingLocation(trainWaitingLocation);
      return GoToClosestWaitingLocation();
    }
    
    public void DeleteEntity()
    {
      if (_currentWaitingLocation != null)
        _currentWaitingLocation.UnOccupy();
    }

    private Decision OccupyWaitingLocation(TrainWaitingLocation trainWaitingLocation)
    {
      if (_currentWaitingLocation != null)
        _currentWaitingLocation.UnOccupy();
      _currentWaitingLocation = trainWaitingLocation;
      if (_currentWaitingLocation == null)
        return Decision.ReleaseNow();
      _currentWaitingLocation.Occupy(gameObject);
      return GoToWaitingLocation(_currentWaitingLocation.TrainDestinationComponent);
    }
    
    private Decision GoToWaitingLocation(TrainDestination trainDestination)
    {
      switch (_moveToStationExecutor.Launch(trainDestination))
      {
        case ExecutorStatus.Success:
          return Decision.ReleaseNow();
        case ExecutorStatus.Failure:
          return Decision.ReleaseNow();
        case ExecutorStatus.Running:
          return Decision.ReturnWhenFinished(_moveToStationExecutor);
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private Decision GoToClosestWaitingLocation()
    {
      var trainWaitingLocation = _closestTrainWaitingLocationPicker.ClosestWaitingLocation(transform.position);
      if (trainWaitingLocation == null)
      {
        _currentWaitingLocation = null;
        return Decision.ReleaseNow();
      }
      return OccupyWaitingLocation(trainWaitingLocation);
    }
  }
}
