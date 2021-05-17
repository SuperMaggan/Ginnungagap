using System.Collections.Generic;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Common.Core.ApplicationServices
{
    public interface IStateService
    {
        /// <summary>
        ///     Serializes the state
        ///     If a state already exists, it will be replaced
        /// </summary>
        /// <param name="state">State to save</param>
        void SaveState(State state);

        /// <summary>
        ///     Deletes the state
        /// </summary>
        /// <param name="name">State to delete</param>
        void DeleteState(string name);

        /// <summary>
        ///     Loads the state.
        /// </summary>
        /// <param name="name">State id to look for</param>
        /// <returns>State, if found. Null if not</returns>
        State LoadState(string name);

        /// <summary>
        ///     Load all states
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        IList<State> LoadStates();
    }
}