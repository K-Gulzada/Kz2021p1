﻿using WebApplication1.EfStuff.Model;

namespace WebApplication1.EfStuff.Repositoryies.PoliceRepositories.Interfaces
{
	public interface IPoliceCallRepo : IBaseRepository<PoliceCallHistory>
    {
        void CreateWithoutSave(PoliceCallHistory policeCall);
        bool SavePoliceCall();
    }
}
