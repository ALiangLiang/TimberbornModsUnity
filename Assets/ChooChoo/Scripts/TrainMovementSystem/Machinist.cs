﻿using Bindito.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BehaviorSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.Persistence;
using Timberborn.TickSystem;
using Timberborn.WalkingSystem;
using UnityEngine;

namespace ChooChoo
{
  public class Machinist : TickableComponent, IPersistentEntity
  {
    private static readonly ComponentKey MachinistKey = new(nameof(Machinist));
    private static readonly PropertyKey<ITrainDestination> CurrentDestinationKey = new("CurrentDestination");
    private static readonly PropertyKey<TrackRoute> LastTrackConnectionKey = new("LastTrackConnection");
    private TrackFollowerFactory _trackFollowerFactory;
    private TrainDestinationObjectSerializer _trainDestinationObjectSerializer;
    private TrackRouteObjectSerializer _trackRouteObjectSerializer;
    private WalkerSpeedManager _walkerSpeedManager;
    private TrainWagonManager _trainWagonManager;
    private SlowdownCalculator _slowdownCalculator;
    private TrackFollower _trackFollower;
    private readonly List<TrackRoute> _pathConnections = new(100);
    private readonly List<TrackRoute> _tempPathCorners = new(100);
    private ITrainDestination _currentTrainDestination;
    private ITrainDestination _previousTrainDestination;
    private float _slowDownDistance = 1.5f;

    public event EventHandler<StartedNewPathEventArgs> StartedNewPath;

    public bool CurrentDestinationReachable { get; private set; }

    public IReadOnlyList<TrackRoute> PathCorners { get; private set; }


    public bool IsStuck { get; set; }

    [Inject]
    public void InjectDependencies(
      TrackFollowerFactory trackFollowerFactory,
      TrainDestinationObjectSerializer trainDestinationObjectSerializer,
      TrackRouteObjectSerializer trackConnectionObjectSerializer
    )
    {
      _trackFollowerFactory = trackFollowerFactory;
      _trainDestinationObjectSerializer = trainDestinationObjectSerializer;
      _trackRouteObjectSerializer = trackConnectionObjectSerializer;
    }

    public void Awake()
    {
      _walkerSpeedManager = GetComponent<WalkerSpeedManager>();
      _trainWagonManager = GetComponent<TrainWagonManager>();
      _slowdownCalculator = GetComponent<SlowdownCalculator>();
      _trackFollower = _trackFollowerFactory.Create(gameObject);
      PathCorners = _pathConnections.AsReadOnly();
    }

    public override void Tick()
    {
      if (Stopped())
        return;
      if (_trackFollower.ReachedLastPathCorner())
        Stop();
      else
        Move();
    }

    public ExecutorStatus GoTo(ITrainDestination trainDestination)
    {
      _previousTrainDestination = _currentTrainDestination;
      int path = (int)FindPath(trainDestination);
      if (path != 2)
        return (ExecutorStatus)path;
      return (ExecutorStatus)path;
    }

    public void Stop()
    {
      _previousTrainDestination = _currentTrainDestination;
      _currentTrainDestination = null;
      _trackFollower.StopMoving();
      _trainWagonManager.StopWagons();
    }

    public bool Stopped() => _currentTrainDestination == null;

    public void RefreshPath()
    {
      _previousTrainDestination = null;
      if (_currentTrainDestination == null)
        return;
      FindPath(_currentTrainDestination);
    }

    public void Save(IEntitySaver entitySaver)
    {
      IObjectSaver component = entitySaver.GetComponent(MachinistKey);
      if (_currentTrainDestination is TrainPositionDestination test && test.Destination != null)
        component.Set(CurrentDestinationKey, _currentTrainDestination, _trainDestinationObjectSerializer);
    }

    public void Load(IEntityLoader entityLoader)
    {
      IObjectLoader component = entityLoader.GetComponent(MachinistKey);
      if (component.Has(CurrentDestinationKey))
        _currentTrainDestination = component.Get(CurrentDestinationKey, _trainDestinationObjectSerializer);
    }

    private ExecutorStatus FindPath(ITrainDestination trainDestination)
    {
      if (!HasSavedPathToDestination(trainDestination))
      {
        _pathConnections.Clear();
        CurrentDestinationReachable = trainDestination.GeneratePath(GetComponent<CharacterModel>().Model, _tempPathCorners, IsStuck);
        if (CurrentDestinationReachable)
        {
          _pathConnections.AddRange(_tempPathCorners);
          _tempPathCorners.Clear();
          _trainWagonManager.SetNewPathConnections(_trackFollower, _pathConnections);
          _slowdownCalculator.SetPositions(_pathConnections[0].RouteCorners[0], _pathConnections.Last().RouteCorners.Last());
          IsStuck = false;
        }
        else
        {
          _pathConnections.Clear();
        }

        _trackFollower.StartMovingAlongPath(_pathConnections);
        EventHandler<StartedNewPathEventArgs> startedNewPath = StartedNewPath;
        if (startedNewPath != null)
          startedNewPath(this, new StartedNewPathEventArgs(100));
      }

      if (CurrentDestinationReachable)
      {
        _currentTrainDestination = trainDestination;
        return !_trackFollower.ReachedLastPathCorner() ? ExecutorStatus.Running : ExecutorStatus.Success;
      }

      Stop();
      return ExecutorStatus.Failure;
    }

    private bool HasSavedPathToDestination(ITrainDestination trainDestination) => Equals(_previousTrainDestination, trainDestination);

    private void Move()
    {
      var speed = _walkerSpeedManager.Speed * _slowdownCalculator.CalculateSlowdown();
      var time = Time.fixedDeltaTime;
      _trackFollower.MoveAlongPath(time, "Walking", speed);
    }
  }
}
