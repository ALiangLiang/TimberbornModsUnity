﻿using Bindito.Core;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;
using Timberborn.TemplateSystem;

namespace ChooChoo
{
  [Configurator(SceneEntrypoint.InGame)]
  public class ChooChooConfigurator : IConfigurator
  {
    public void Configure(IContainerDefinition containerDefinition)
    {
      containerDefinition.Bind<ChooChooCore>().AsSingleton();
    }
  }
}
