using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Entity;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.EntityNode.Operations;

public interface IDomainMasterManagerFactory
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="server"></param>
    /// <param name="applicationConfiguration"></param>
    /// <returns></returns>
    public DomainMasterNodeManager ConstructMasterManager(IServerInternal server, ApplicationConfiguration applicationConfiguration);
}


public sealed class DomainMasterNodeManager : MasterNodeManager
{
    public IEnumerable<IEntityNode> ManagedEntities { get; private set; }

    /// <inheritdoc />
    public DomainMasterNodeManager(IServerInternal server, ApplicationConfiguration applicationConfiguration,
        IEntityNodeManager[] additionalManagers) : base(server, applicationConfiguration,
        applicationConfiguration.ApplicationUri, additionalManagers)
    {
        ManagedEntities = additionalManagers.Select(e => e.ManagedEntity);
    }



    /// <inheritdoc />
    public override void RegisterNodes(OperationContext context, NodeIdCollection nodesToRegister,
        out NodeIdCollection registeredNodeIds)
    {
        base.RegisterNodes(context, nodesToRegister, out registeredNodeIds);
    }

    /// <inheritdoc />
    public override void UnregisterNodes(OperationContext context, NodeIdCollection nodesToUnregister)
    {
        base.UnregisterNodes(context, nodesToUnregister);
    }


    /// <inheritdoc />
    public override void BrowseNext(OperationContext context, bool releaseContinuationPoints,
        ByteStringCollection continuationPoints,
        out BrowseResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
    {
        base.BrowseNext(context, releaseContinuationPoints, continuationPoints, out results, out diagnosticInfos);
    }

    /// <inheritdoc />
    public override void Read(OperationContext context, double maxAge, TimestampsToReturn timestampsToReturn,
        ReadValueIdCollection nodesToRead, out DataValueCollection values, out DiagnosticInfoCollection diagnosticInfos)
    {
        base.Read(context, maxAge, timestampsToReturn, nodesToRead, out values, out diagnosticInfos);
    }

    /// <inheritdoc />
    public override void HistoryRead(OperationContext context, ExtensionObject historyReadDetails,
        TimestampsToReturn timestampsToReturn,
        bool releaseContinuationPoints, HistoryReadValueIdCollection nodesToRead,
        out HistoryReadResultCollection results,
        out DiagnosticInfoCollection diagnosticInfos)
    {
        base.HistoryRead(context, historyReadDetails, timestampsToReturn, releaseContinuationPoints, nodesToRead,
            out results, out diagnosticInfos);
    }

    /// <inheritdoc />
    public override void Write(OperationContext context, WriteValueCollection nodesToWrite,
        out StatusCodeCollection results,
        out DiagnosticInfoCollection diagnosticInfos)
    {
        base.Write(context, nodesToWrite, out results, out diagnosticInfos);
    }

    /// <inheritdoc />
    public override void HistoryUpdate(OperationContext context, ExtensionObjectCollection historyUpdateDetails,
        out HistoryUpdateResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
    {
        base.HistoryUpdate(context, historyUpdateDetails, out results, out diagnosticInfos);
    }

    /// <inheritdoc />
    public override void Call(OperationContext context, CallMethodRequestCollection methodsToCall,
        out CallMethodResultCollection results,
        out DiagnosticInfoCollection diagnosticInfos)
    {
        base.Call(context, methodsToCall, out results, out diagnosticInfos);
    }

    /// <inheritdoc />
    public override void ConditionRefresh(OperationContext context, IList<IEventMonitoredItem> monitoredItems)
    {
        base.ConditionRefresh(context, monitoredItems);
    }

    /// <inheritdoc />
    public override void CreateMonitoredItems(OperationContext context, uint subscriptionId, double publishingInterval,
        TimestampsToReturn timestampsToReturn, IList<MonitoredItemCreateRequest> itemsToCreate,
        IList<ServiceResult> errors, IList<MonitoringFilterResult> filterResults,
        IList<IMonitoredItem> monitoredItems)
    {
        base.CreateMonitoredItems(context, subscriptionId, publishingInterval, timestampsToReturn, itemsToCreate,
            errors, filterResults, monitoredItems);
    }

    /// <inheritdoc />
    public override void ModifyMonitoredItems(OperationContext context, TimestampsToReturn timestampsToReturn,
        IList<IMonitoredItem> monitoredItems,
        IList<MonitoredItemModifyRequest> itemsToModify, IList<ServiceResult> errors,
        IList<MonitoringFilterResult> filterResults)
    {
        base.ModifyMonitoredItems(context, timestampsToReturn, monitoredItems, itemsToModify, errors, filterResults);
    }

    /// <inheritdoc />
    public override void TransferMonitoredItems(OperationContext context, bool sendInitialValues,
        IList<IMonitoredItem> monitoredItems, IList<ServiceResult> errors)
    {
        base.TransferMonitoredItems(context, sendInitialValues, monitoredItems, errors);
    }

    /// <inheritdoc />
    public override void DeleteMonitoredItems(OperationContext context, uint subscriptionId,
        IList<IMonitoredItem> itemsToDelete, IList<ServiceResult> errors)
    {
        base.DeleteMonitoredItems(context, subscriptionId, itemsToDelete, errors);
    }

    /// <inheritdoc />
    public override void SetMonitoringMode(OperationContext context, MonitoringMode monitoringMode,
        IList<IMonitoredItem> itemsToModify, IList<ServiceResult> errors)
    {
        base.SetMonitoringMode(context, monitoringMode, itemsToModify, errors);
    }
}