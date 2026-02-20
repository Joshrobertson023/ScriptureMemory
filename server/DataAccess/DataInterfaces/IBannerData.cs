using DataAccess.Models;

namespace DataAccess.DataInterfaces;

public interface IBannerData
{
    Task<SiteBanner?> GetBanner();
    Task SetBanner(SiteBanner banner);
    Task RemoveBanner();
}

