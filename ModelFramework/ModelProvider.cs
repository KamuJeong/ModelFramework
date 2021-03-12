using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace Kamu.ModelFramework
{
    public abstract class ModelProvider
    {
        #region [Provider primitives]

        public Uri Uri { get; private set; }

        protected ModelInventory Models { get; private set; }

        public bool IsOpened { get; private set; }

        public bool IsClosed { get; private set; }

        private ConcurrentExclusiveSchedulerPair _schedulerPair = new ConcurrentExclusiveSchedulerPair();

        protected ModelProvider()
        {
        }

        protected ValueTask InvokeAsync(Action action, bool exclusive = true)
        {
            return new ValueTask(Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                                                exclusive ? _schedulerPair.ExclusiveScheduler : _schedulerPair.ConcurrentScheduler));
        }

        protected ValueTask<T> InvokeAsync<T>(Func<T> function, bool exclusive = true)
        {
            return new ValueTask<T>(Task.Factory.StartNew(function, CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                                                exclusive ? _schedulerPair.ExclusiveScheduler : _schedulerPair.ConcurrentScheduler));
        }

        private async ValueTask<bool> OpenAync() => IsOpened = await InvokeAsync(Opening);

        protected abstract bool Opening();

        internal void Close()
        {
            if (!IsClosed)
            {
                Closing();
                IsOpened = false;
                IsClosed = true;
                _schedulerPair.Complete();
            }
        }

        protected abstract void Closing();

        public void Abort() => Models.Abort(this);

        #endregion

        #region [Model primitives]

        internal protected abstract Model Create(string query);

        internal void Delete(Model model) => Models.Delete(model);

        public bool Load(Model model)
        {
            if (IsClosed) return false;

            if (!IsOpened && !Opening())
            {
                return false;
            }

            if (Loading(model))
            {
                InvokeChanged(model, ChangingSource.Load);
                return true;
            }

            return false;
        }

        public async ValueTask<bool> LoadAsync(Model model)
        {
            if (IsClosed) return false;

            if (!IsOpened && !IsClosed && !await OpenAync())
            {
                return false;
            }

            if (await InvokeAsync(() => Loading(model)))
            {
                InvokeChanged(model, ChangingSource.Load);
                return true;
            }

            return false;
        }

        protected abstract bool Loading(Model model);

        public bool Save(Model model)
        {
            if (IsClosed || !model.IsLoaded) return false;

            return Saving(model);
        }

        public ValueTask<bool> SaveAsync(Model model)
        {
            if (IsClosed || !model.IsLoaded) return new ValueTask<bool>(false);

            return InvokeAsync(() => Saving(model));
        }

        protected abstract bool Saving(Model model);

        #endregion

        #region [Helpers]

        protected void InvokeChanged(Model model, ChangingSource source) => model.OnChanged(source);

        #endregion
    }
}
