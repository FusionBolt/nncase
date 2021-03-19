/* Copyright 2020 Canaan Inc.
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
#pragma once
#include "plugin_loader.h"
#include <cstddef>
#include <cstdint>
#include <filesystem>
#include <iostream>
#include <memory>
#include <span>
#include <unordered_map>
#include <vector>

namespace nncase
{
struct compile_options
{
    bool dump_ir;
    bool dump_asm;
    std::string target;
    std::filesystem::path dump_dir;
};

struct import_options
{
    std::span<const std::string> output_arrays;
};

struct ptq_dataset_options
{
    std::filesystem::path dataset;
    std::string dataset_format;
    std::function<void(size_t cnt, size_t total)> progress;

    float input_mean = 0.f;
    float input_std = 1.f;
};

struct ptq_tensor_options
{
    std::vector<uint8_t> tensor_data;
    size_t samples_count;
    std::function<void(size_t cnt, size_t total)> progress;
};

class NNCASE_API compiler
{
public:
    static std::unique_ptr<compiler> create(const compile_options &options);

    virtual ~compiler();
    virtual void import_tflite(std::span<const uint8_t> model, const import_options &options) = 0;
    virtual void use_ptq(ptq_dataset_options options) = 0;
    virtual void use_ptq(ptq_tensor_options options) = 0;
    virtual void compile() = 0;
    virtual void gencode(std::ostream &output) = 0;
};
}