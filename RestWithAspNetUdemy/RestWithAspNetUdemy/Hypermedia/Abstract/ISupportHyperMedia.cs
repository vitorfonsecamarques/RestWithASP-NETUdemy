﻿namespace RestWithAspNetUdemy.Hypermedia.Abstract
{
    public interface ISupportHyperMedia
    {
        List<HyperMediaLink> Links { get; set; }
    }
}
