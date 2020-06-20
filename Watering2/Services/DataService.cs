using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Watering2.DbContext;
using Watering2.Models;

namespace Watering2.Services
{
    public class DataService : INotifyPropertyChanged
    {
        private const int MaxHistoryDays = 14;
        private object _lock = new object();

        public void SaveReadingPoint(Measurement readingPoint)
        {
            bool lockTaken = false;
            Monitor.TryEnter(_lock, new TimeSpan(0, 2, 0), ref lockTaken);

            if (!lockTaken)
            {
                //Terminal.Settings.DisplayLoggingMessageType = LogMessageType.Info | LogMessageType.Warning | LogMessageType.Error | LogMessageType.Fatal | LogMessageType.Debug;
                //$"Lock not taken".Warn("SaveReadingData", null, string.Empty);
                return;
            }

            using (var context = new WateringContext())
            {
                context.Measurements.Add(readingPoint);
                context.SaveChanges();
            }

            Monitor.Exit(_lock);
            OnPropertyChanged(nameof(readingPoint));
        }

        public List<Measurement> GetReadingPointsByStartAndEndTime(DateTime startTime, DateTime endTime)
        {
            bool lockTaken = false;
            Monitor.TryEnter(_lock, new TimeSpan(0, 2, 0), ref lockTaken);

            if (!lockTaken)
            {
                //Terminal.Settings.DisplayLoggingMessageType = LogMessageType.Info | LogMessageType.Warning | LogMessageType.Error | LogMessageType.Fatal | LogMessageType.Debug;
                //$"Lock not taken".Warn("GetReadingPointsByStartAndEndTime", $"start: {startTime:d/M/yy hh:mm} end: {endTime:d/M/yy hh:mm}", string.Empty);
                return new List<Measurement>();
            }

            using var context = new WateringContext();
            List<Measurement> resultList = context.Measurements
                .Where(p => p.TimeStamp >= startTime && p.TimeStamp <= endTime).AsNoTracking().ToList();
            Monitor.Exit(_lock);
            return resultList;
        }

        public IEnumerable<Measurement> GetReadingPointsFirst(int number)
        {
            bool lockTaken = false;
            Monitor.TryEnter(_lock, new TimeSpan(0, 2, 0), ref lockTaken);

            if (!lockTaken)
            {
                //Terminal.Settings.DisplayLoggingMessageType = LogMessageType.Info | LogMessageType.Warning | LogMessageType.Error | LogMessageType.Fatal | LogMessageType.Debug;
                //$"Lock not taken".Warn("GetReadingPointsFirst", number, string.Empty);
                return new List<Measurement>();
            }

            using var context = new WateringContext();
            int cntRows = context.Measurements.Count();
            int skip = cntRows - number;
            if (skip > 0)
            {
                List<Measurement> tmpList = context.Measurements.OrderByDescending(x => x.TimeStamp).Take(number).ToList(); //
                Monitor.Exit(_lock);
                return tmpList;
                //return tmpList.GetRange(0, number);
            }

            List<Measurement> result = context.Measurements.OrderByDescending(x => x.TimeStamp).ToList();
            Monitor.Exit(_lock);
            return result;
        }

        public IEnumerable<WateringData> GetTracePointsFirst(int number)
        {
            bool lockTaken = false;
            Monitor.TryEnter(_lock, new TimeSpan(0, 2, 0), ref lockTaken);

            if (!lockTaken)
            {
                //Terminal.Settings.DisplayLoggingMessageType = LogMessageType.Info | LogMessageType.Warning | LogMessageType.Error | LogMessageType.Fatal | LogMessageType.Debug;
                //$"Lock not taken".Warn("GetTracePointsFirst", number, string.Empty);
                return new List<WateringData>();
            }

            List<WateringData> results;

            using (var context = new WateringContext())
            {
                int cntRows = context.Waterings.Count();
                int skip = cntRows - number;
                if (skip > 0)
                {
                    results = context.Waterings.Skip(skip).OrderByDescending(x => x.TimeStamp).ToList();
                    Monitor.Exit(_lock);
                    return results;
                }

                results = context.Waterings.OrderByDescending(x => x.TimeStamp).ToList();
                Monitor.Exit(_lock);
                return results;
            }
        }

        public bool EmergencyWateringNecessary(int days)
        {
            if (days <= 0) return false; // 0: Function disabled
            bool lockTaken = false;
            Monitor.TryEnter(_lock, new TimeSpan(0, 2, 0), ref lockTaken);

            if (!lockTaken)
            {
                //Terminal.Settings.DisplayLoggingMessageType = LogMessageType.Info | LogMessageType.Warning | LogMessageType.Error | LogMessageType.Fatal | LogMessageType.Debug;
                //$"Lock not taken".Warn("EmergencyWateringNecessary", days, string.Empty);
                return false;
            }

            using var context = new WateringContext();
            List<WateringData> lastWatering = context.Waterings.Where(p => p.Duration > 0).OrderByDescending(t => t.TimeStamp).AsNoTracking().ToList();
            Monitor.Exit(_lock);

            if (lastWatering.Count == 0)
                return true;

            return (DateTime.Now - lastWatering[0].TimeStamp).TotalDays > days;
        }

        private void DeleteDataPointsFromDate(DateTime deleteOlderThan)
        {
            bool lockTaken = false;
            Monitor.TryEnter(_lock, new TimeSpan(0, 2, 0), ref lockTaken);

            if (!lockTaken)
            {
                //Terminal.Settings.DisplayLoggingMessageType = LogMessageType.Info | LogMessageType.Warning | LogMessageType.Error | LogMessageType.Fatal | LogMessageType.Debug;
                //$"Lock not taken".Warn("DeleteDataPointsFromDate", $"{deleteOlderThan:d/M/yy hh:mm}", string.Empty);
                return;
            }

            using var context = new WateringContext();
            bool wasChanged = false;

            List<Measurement> toDeleteReadingPoints = context.Measurements.Where(x => x.TimeStamp < deleteOlderThan).ToList();
            if (toDeleteReadingPoints.Count > 0)
            {
                context.Measurements.RemoveRange(toDeleteReadingPoints);
                wasChanged = true;
            }

            List<WateringData> toDeleteTraceData = context.Waterings.Where(x => x.TimeStamp < deleteOlderThan).AsNoTracking().ToList();
            if (toDeleteTraceData.Count > 0)
            {
                context.Waterings.RemoveRange(toDeleteTraceData);
                wasChanged = true;
            }

            if (wasChanged)
                context.SaveChanges();

            Monitor.Exit(_lock);
        }

        public void SaveWateringData(WateringData wateringData)
        {
            bool lockTaken = false;
            Monitor.TryEnter(_lock, new TimeSpan(0, 2, 0), ref lockTaken);

            if (!lockTaken)
            {
                //Terminal.Settings.DisplayLoggingMessageType = LogMessageType.Info | LogMessageType.Warning | LogMessageType.Error | LogMessageType.Fatal | LogMessageType.Debug;
                //$"Lock not taken".Error("SaveWateringData", null, string.Empty);
                return;
            }

            using (var context = new WateringContext())
            {
                context.Waterings.Add(wateringData);
                context.SaveChanges();
            }
            Monitor.Exit(_lock);

            DateTime now = DateTime.Now;
            now = now.AddDays(-MaxHistoryDays);
            DeleteDataPointsFromDate(now);
            OnPropertyChanged(nameof(wateringData));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
