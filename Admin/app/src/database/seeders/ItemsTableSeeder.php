<?php

namespace Database\Seeders;

use App\Models\Account;
use App\Models\Item;
use Illuminate\Database\Console\Seeds\WithoutModelEvents;
use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\Hash;

class ItemsTableSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {
        //
        Item::create([
            'name' => '回復薬',
            'type' => '消耗品',
            'effect_size' => 1,
            'Description' => '少し回復することができる'
        ]);
        Item::create([
            'name' => '仙豆',
            'type' => '消耗品',
            'effect_size' => 3,
            'Description' => '体力を全回復する'
        ]);
    }
}
