﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.DTOs
{
    public record AddressDto(
        string Street,
        string City,
        string Governorate,
        string PostalCode
    );

}
