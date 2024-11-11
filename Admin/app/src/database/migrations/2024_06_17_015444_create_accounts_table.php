<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration {
    /**
     * Run the migrations.
     */
    public function up(): void
    {
        Schema::create('accounts', function (Blueprint $table) {
            $table->id();//idカラム
            $table->string('name', 20);//nameカラム
            $table->string('password');//passwordカラム
            $table->timestamps();//created_at,updated_at

            $table->index('name');//indexの設定
            $table->unique('name');//ユニーク制約
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('accounts');
    }
};
