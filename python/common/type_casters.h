/* Copyright 2019-2021 Canaan Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#include <nncase/compiler_defs.h>
#include <pybind11/pybind11.h>

namespace pybind11
{
namespace detail
{
    template <>
    struct type_caster<gsl::span<const gsl::byte>>
    {
    public:
        PYBIND11_TYPE_CASTER(gsl::span<const gsl::byte>, _("bytes"));

        bool load(handle src, bool)
        {
            if (!py::isinstance<py::bytes>(src))
                return false;

            uint8_t *buffer;
            py::ssize_t length;
            if (PyBytes_AsStringAndSize(src.ptr(), reinterpret_cast<char **>(&buffer), &length))
                return false;
            value = { (const gsl::byte *)buffer, (size_t)length };
            loader_life_support::add_patient(src);
            return true;
        }
    };
}
}
