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

        public bool IsOpened { get; protected set; }

        public bool IsClosed { get; private set; }

        protected virtual ValueTask InvokeAsync(Action action, TaskScheduler scheduler = null)
        {
            return new ValueTask(Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                                                scheduler ?? TaskScheduler.Current));
        }

        protected virtual ValueTask<T> InvokeAsync<T>(Func<T> function, TaskScheduler scheduler = null)
        {
            return new ValueTask<T>(Task.Factory.StartNew(function, CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                                                scheduler ?? TaskScheduler.Current));
        }


        protected ModelProvider()
        {
        }

        protected bool Open() => IsOpened = Opening();

        protected async virtual ValueTask<bool> OpenAync() => IsOpened = await InvokeAsync(Opening);

        protected abstract bool Opening();

        internal void Close()
        {
            if (!IsClosed)
            {
                Closing();
                IsOpened = false;
                IsClosed = true;
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

            if (!IsOpened && !Open())
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

        public async virtual ValueTask<bool> LoadAsync(Model model)
        {
            if (IsClosed) return false;

            if (!IsOpened && !IsClosed && !await OpenAync())
            {
                return false;
            }

            var temp = Create(model.Uri.Model());
            if (await InvokeAsync(() => Loading(temp)))
            {
                model.CopyFrom(temp);
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

        public async virtual ValueTask<bool> SaveAsync(Model model)
        {
            if (IsClosed || !model.IsLoaded) return false;

            var temp = Create(model.Uri.Model());
            temp.CopyFrom(model);

            return await InvokeAsync(() => Saving(temp));
        }

        protected abstract bool Saving(Model model);

        #endregion

        #region [Helpers]

        protected void InvokeChanged(Model model, ChangingSource source) => model.OnChanged(source);

        #endregion
    }
}
