﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Extensions
{
    public enum TaskOption
    {
        None,
        Safe
    }

    public static class TaskExtensions
    {
        /// <summary>
        /// Task will be awaited and exceptions will be managed by the Blazor framework.
        /// </summary>
        public static async void AndForget(this Task task)
        {
            await task;
        }

        /// <summary>
        /// Task will be awaited and exceptions will be logged to console (TaskOption.Safe) or managed by the Blazor framework (TaskOption.None).
        /// </summary>
        public static async void AndForget(this Task task, TaskOption option)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (option != TaskOption.Safe)
                    throw;

                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// ValueTask will be awaited and exceptions will be managed by the Blazor framework.
        /// </summary>
        public static async void AndForget(this ValueTask task)
        {
            await task;
        }

        /// <summary>
        /// ValueTask will be awaited and exceptions will be logged to console (TaskOption.Safe) or managed by the Blazor framework (TaskOption.None).
        /// </summary>
        public static async void AndForget(this ValueTask task, TaskOption option)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (option != TaskOption.Safe)
                    throw;

                Console.WriteLine(ex);
            }
        }
    }
}
